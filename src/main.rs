use core::fmt;
use std::{collections::HashMap, io::{stdout, Read, Write}, isize, net::{IpAddr, Shutdown, TcpListener, TcpStream}, thread, u128, usize};

use log::{debug, error, info, trace};
use serde::{de::Error, Serialize};
use simple_logger::SimpleLogger;
use types::varint::{self, ivar};
use std::sync::{Arc, Mutex};

mod types;

struct Server<'a> {
    address: &'a str,
    connections: HashMap<String, TcpStream>,
    players: HashMap<String, Player>,
}

impl Server<'_> {
    pub fn new(address: &'static str) -> Self {
        Self {
            address,
            connections: HashMap::new(),
            players: HashMap::new(),
        }
    }
}

struct Connection {
    stream: TcpStream,
    ip: IpAddr,
    state: ConnectionState,
}

impl Connection {
    pub fn new(stream: TcpStream) -> Result<Self, ()> {
        let ip = match stream.local_addr() {
            Ok(addr) => addr.ip(),
            Err(_) => {
                error!("Disconnecting, Reason: Could not establish connection IP!");
                let _ = stream.shutdown(std::net::Shutdown::Both);
                return Err(());
            },
        };
        Ok(Self {
            stream,
            ip,          
            state: ConnectionState::Handshake, // Will always start with a handshake
        })
    } 

    pub fn read(&mut self, buffer: &mut [u8]) -> Result<usize, std::io::Error> {
        self.stream.read(buffer)
    }

    pub fn shutdown(&mut self,how: Shutdown) -> Result<(), std::io::Error>{
        self.stream.shutdown(how)
    }

    pub fn set_state(&mut self, state: ConnectionState) {
        self.state = state;
    }

    pub fn get_state(&self) -> &ConnectionState {
        &self.state
    }
}

struct Player {
    connection: Connection,
    name: String,
    uuid: u128,
}

impl Player {
    pub fn new(connection: Connection, name: String, uuid: u128) -> Self {
        Self {
            connection,
            name,
            uuid
        }
    }

    pub fn uuid(&self) -> u128 {
        self.uuid
    }
    
    pub fn name(&self) -> String {
        self.name.clone()
    }
}

fn main() {
    let server = Server::new("127.0.0.1:25565");
    
    let listener = match TcpListener::bind(server.address) {
        Ok(l) => l,
        Err(e) => panic!("{e:?}"),
    };

    let mut handles = Vec::new();
    
    SimpleLogger::new().init().unwrap();

    for stream in listener.incoming() {
        match stream {
            Ok(s) => handles.push(thread::spawn(move || start_connection(s))),
            Err(e) => println!("{e:?}"),
        }
    }

    for handle in handles {
        match handle.join() {
            Err(e) => println!("{e:?}"),
            _ => (),
        }
    }
}

fn start_connection(stream: TcpStream) {
    match stream.local_addr() {
        Ok(addr) => info!("Starting connection with: {}", addr.ip()),
        Err(e) => {
            error!("Could not get ip from client!\n{e:?}");
            return;
        },
    }
    let mut connection = match Connection::new(stream) {
        Ok(conn) => conn,
        Err(e) => {
            error!("{e:?}");
            return;
        }
    };

    loop {
        let mut buf: [u8; 4096] = [0; 4096];
        let _ = connection.read(&mut buf);
        if buf[0] == 0 {
            break;
        }
        
        // First value (variable length integer) is length of packet ID + data byte array
        // So lets cut down the 4096 byte array to just the relevant data
        let length_ivar = ivar::read(&buf).unwrap();
        let length = length_ivar.length();
        let buf = &buf[length..=length_ivar.value as usize];
        
        let packet_id_ivar = ivar::read(buf).unwrap();
        let packet_id = packet_id_ivar.value;

        info!("Packet ID: {:#x?}", packet_id);
        
        debug!("Connection {}, State: {}", connection.ip, connection.state);

        let buf = &buf[packet_id_ivar.length()..];
        // Packet ID matching
        match connection.state {
            ConnectionState::Handshake => {
                match packet_id {
                    0x00 => {
                        handshake(&mut connection, buf);
                    },
                    0xFE => {
                        info!("Legacy ping detected, IP: {}", connection.ip);
                    },
                    _ => {
                        debug!("Buffer text: {}", convert_buf_to_string(buf));
                        debug!("Buffer: {:?}", buf);
                        error!("Unrecognised packet")
                    },
                } 
            },
            ConnectionState::Login => {
                match packet_id {
                    0x00 => {
                        match login(buf) {
                            Ok((name, uuid)) => {
                                match login_success(connection, name, uuid) {
                                    Ok(player) => loop {},
                                    Err(e) => error!("Error with login success!: {}", e),
                                }
                                break;
                            },
                            Err(e) => error!("{e}"),
                        };
                    },
                    _ => unimplemented!(),
                }
            },
            ConnectionState::Status => {
                status(&mut connection.stream);
            },
            _ => unimplemented!(),
        }
    }
}

fn handshake(connection: &mut Connection, buffer: &[u8]) {
    match connection.stream.local_addr() {
        Ok(addr) => info!("Starting handshake with: {}", addr.ip()),
        Err(e) => {
            error!("Could not get ip from client!\n{e:?}");
            return;
        },
    }

    let protocol_varint: ivar = match ivar::read(&buffer) {
        Ok(e) => e,
        Err(e) => {
            error!("{buffer:?}\n{e:?}");
            ivar::new(1000)
        },
    };

    let state: ConnectionState = ConnectionState::from_u8(*buffer.last().unwrap());
    debug!("Protocl: {}", protocol_varint.value);
    
    match state {
        ConnectionState::Unknown => {
            match connection.stream.local_addr() {
                Ok(addr) => info!("Disconnecting {}, Reason: Unknown handshake state!", addr.ip()),
                Err(e) => error!("Could not get ip from client!\n{e:?}"),
            }
            let _ = connection.shutdown(std::net::Shutdown::Both);
            return;
        },
        _ => connection.state = state,
    }
}

fn status(stream: &mut TcpStream) {
    let x = StatusResponse::new();
    let packet_id = ivar::new(0).as_bytes();
    let response_string = serde_json::to_string(&x).unwrap();
    let mut buffer: Vec<u8> = Vec::new();
    
    buffer.extend_from_slice(&packet_id);
    write_utf8_string(&mut buffer, response_string);

    let length = ivar::new(buffer.len() as i32).as_bytes();
    let _ = stream.write_all(&length);
    let _ = stream.write_all(&buffer);
}

fn write_utf8_string(buffer: &mut Vec<u8>, text: String) {
    let mut data: Vec<u8> = text.into_bytes();
    buffer.extend_from_slice(&ivar::new(data.len() as i32).as_bytes());
    buffer.append(&mut data);
}

fn login(buffer: &[u8]) -> Result<(String, u128), &'static str>{
    // Login Start Packet 
    // 0x00 Login Name (string 16) Player UUID (u128)
    let string_ivar = ivar::read(buffer).unwrap();
    
    let player_name_bytes = &buffer[..=string_ivar.value as usize];
    let tmp_buf = buffer[buffer.len()-std::mem::size_of::<u128>()..].iter().map(|x| *x).collect::<Vec<u8>>();
    let byte_array: [u8; 16] = tmp_buf.try_into().unwrap();

    let player_name = convert_buf_to_string(player_name_bytes);
    let uuid = u128::from_be_bytes(byte_array);
    info!("Connecting: {} ({:#x})", player_name, uuid); 
    Ok((player_name, uuid))
}

fn login_success(connection: Connection, name: String, uuid: u128) -> Result<Player, &'static str>{
   
    debug!("Constructing login success packet");
    let mut player = Player::new(connection, name, uuid);

    let packet_id = ivar::new(0x02).as_bytes();
    let uuid = player.uuid().to_be_bytes();
    let name = player.name();
    let name = name.as_bytes();

    let num_of_properties = ivar::new(0).as_bytes();
    let property: [u8; 0] = [];
    let error_handling: bool = true;

    let mut bytes: Vec<u8> = Vec::new();

    bytes.extend_from_slice(&packet_id);
    bytes.extend_from_slice(&uuid);
    bytes.extend_from_slice(&name);
    bytes.extend_from_slice(&num_of_properties);
    bytes.push(0x1);


    let mut new_bytes: Vec<u8> = Vec::new();
    new_bytes.extend_from_slice(&ivar::new(bytes.len() as i32).as_bytes());

    new_bytes.extend_from_slice(&bytes);

    debug!("Writing packet\n {:?}", new_bytes);
    player.connection.stream.write_all(&new_bytes);
    debug!("Sent packets!");
    Ok(player)
}

#[derive(Debug, Serialize)]
enum ConnectionState {
    Handshake = 0,
    Status = 1,
    Login = 2,
    Transfer = 3,
    Unknown = 4,
}

impl ConnectionState {
    fn from_u8(value: u8) -> ConnectionState {
        match value {
            0 => Self::Handshake,
            1 => Self::Status,
            2 => Self::Login,
            3 => Self::Transfer,
            _ => {
                error!("Unknown connection state: {}!", value);
                Self::Unknown
            } 
        }
    }
}

impl fmt::Display for ConnectionState {
    fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
        match self {
            Self::Handshake => write!(f, "Handshake"),
            Self::Status => write!(f, "Status"),
            Self::Login => write!(f, "Login"),
            Self::Transfer => write!(f, "Transfer"),
            Self::Unknown => write!(f, "Unknown"),
        }
    }
}

fn convert_buf_to_string(buff: &[u8]) -> String {
    let mut constructed_string = String::new();
    // println!("{:?}", buff);
    for byte in buff {
        constructed_string = format!("{}{}", constructed_string, *byte as char);
    }
    constructed_string
}


#[derive(Debug, Serialize)]
struct StatusResponse<'a> {
    version: Version <'a>,
    players: Players ,
    description: Description <'a>,
    favicon: &'static str,
}

#[derive(Debug, Serialize)]
struct Version <'a> {
    name: &'a str,
    protocol: usize,
}

#[derive(Debug, Serialize)]
struct DisplayPlayer {
    name: &'static str,
    id: &'static str,
}

#[derive(Debug, Serialize)]
struct Players {
    max: usize,
    online: usize,
    sample: Vec<DisplayPlayer>,
}

#[derive(Debug, Serialize)]
struct Description<'a> {
    text: &'a str,
}

impl StatusResponse<'_> {
    pub fn new() -> Self {
        Self {
            version: Version {
                name: "1.21.1",
                protocol: 767,
            },
            players: Players {
                max: 100,
                online: 12,
                sample: vec!(DisplayPlayer {
                    name: "thinkofdeath",
                    id: "4566e69f-c907-48ee-8d71-d7ba5aa00d20"
                }),
            },
            description: Description {
                text: "OwO",
            },
            favicon: "data:image/png;base64,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",
        }
    }
}
