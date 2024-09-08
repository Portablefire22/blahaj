use core::fmt;
use std::{borrow::Borrow, collections::HashMap, io::{stdout, Read, Write}, isize, net::{IpAddr, Shutdown, TcpListener, TcpStream}, thread, u128, usize};

use connection::ConnectionState;
use fastnbt::SerOpts;
use log::{debug, error, info, trace};
use registry::biomes::Biome;
use registry_data::{construct_registry_packet, send_registry_packet, RegistryEntry};
use serde::{de::Error, Serialize};
use simple_logger::SimpleLogger;
use types::varint::{self, ivar, VarIntDecodeError};
use utils::{write_ivar, write_utf8_string};
use std::sync::{Arc, Mutex};

mod types;
mod status_response;
mod connection;
mod utils;
mod registry_data;
mod registry;

use crate::{status_response::StatusResponse, connection::Connection};


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
        let (buf, length) = connection.read(&mut buf).unwrap();
        if buf[0] == 0 {
            break;
        }
        let buf = &buf;       
        
        let raw_buffer = buf; // Some packets can be sent back
        
        let buf = &buf[length..];

        let packet_id_ivar = ivar::read(buf).unwrap();
        let packet_id = packet_id_ivar.value;

        info!("Packet ID: {:#x?}", packet_id);
        
        debug!("Connection {}, State: {}", connection.ip(), connection.get_state());

        let buf = &buf[packet_id_ivar.length()..];
        // Packet ID matching
        match connection.get_state() {
            ConnectionState::Handshake => {
                match packet_id {
                    0x00 => {
                        handshake(&mut connection, buf);
                    },
                    0xFE => {
                        info!("Legacy ping detected, IP: {}", connection.ip());
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
                                    Ok(mut player) => {
                                        // clientbound_pack(&mut player);
                                        registry_data(&mut player);
                                        // let _ = player.connection.get_stream().write_all(&ivar::new(0x03).as_bytes());
                                        loop {
                                            let mut buf: [u8; 4096] = [0; 4096];
                                            let _ = player.connection.read(&mut buf);
                                            if buf[0] == 0 {
                                                break;
                                            }
                                            // debug!("{:?}", convert_buf_to_string(&buf));
                                        }
                                    },
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
                match packet_id {
                    0x00 => {
                        status(&mut connection.get_stream());
                    },
                    0x01 => {
                        ping(&mut connection.get_stream(), &raw_buffer);
                    }
                    _ => unimplemented!(),
                }
            },
            _ => unimplemented!(),
        }
    }
}

fn handshake(connection: &mut Connection, buffer: &[u8]) {
    match connection.get_stream().local_addr() {
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
            let msg = match connection.get_stream().local_addr() {
                Ok(addr) => String::from("Unknown handshake state!"),
                Err(e) => format!("Could not get ip from client!\n{e:?}").to_string(),
            };
            let _ = connection.shutdown(std::net::Shutdown::Both, Some(msg));
            return;
        },
        _ => connection.set_state(state),
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

fn ping(stream: &mut TcpStream, data: &[u8]) {
    debug!("{data:?}");
    let _ = stream.write_all(data);
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
    player.connection.get_stream().write_all(&new_bytes);
    debug!("Sent packets!");
    Ok(player)
}

fn registry_data(player: &mut Player) {
    // https://wiki.vg/Protocol#Registry_Data
    // https://gist.github.com/WinX64/ab8c7a8df797c273b32d3a3b66522906
    
    let biome = Biome::default();
    let entry = RegistryEntry {
        entry_id: "minecraft:plains".into(),
        has_data: true,
        data: Some(fastnbt::to_bytes_with_opts(&biome, SerOpts::network_nbt()).unwrap()),
    };

    let reg = construct_registry_packet("minecraft:worldgen/biome".into(), vec!(entry));
    debug!("{}", reg.len());
    send_registry_packet(player.connection.get_stream(), &reg);
}

fn clientbound_pack(player: &mut Player) {
    let mut buf: Vec<u8> = Vec::new();
    buf.push(0x0E);
    write_ivar(&mut buf, 0x01);
    // let x = vec!("minecraft".as_bytes(), "core".as_bytes(), "1.21".as_bytes());
    buf.extend_from_slice(&"minecraft".as_bytes());
    buf.extend_from_slice(&"core".as_bytes());
    buf.extend_from_slice(&"1.21".as_bytes());
    // buf.extend_from_slice(&x); 
    debug!("Client bound: {buf:?}");
    send_buffer(&player.connection.get_stream(), &buf);
}

fn send_buffer(mut stream: &TcpStream, buffer: &[u8]) {
    send_length(stream, buffer);
    let _ = stream.write_all(&buffer);
}

fn send_length(mut stream: &TcpStream, buffer: &[u8]) {
    let length = ivar::new(buffer.len() as i32);
    let _ = stream.write_all(&length.as_bytes());
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



