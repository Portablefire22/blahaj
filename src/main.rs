use core::fmt;
use std::{io::{stdout, Read, Write}, isize, net::{IpAddr, TcpListener, TcpStream}, thread, usize};

use log::{debug, error, info};
use serde::Serialize;
use simple_logger::SimpleLogger;
use types::varint::{self, ivar};

mod types;

const ADDR: &'static str= "127.0.0.1:25565";

struct Server<'a> {
    address: &'a str,
    connections: Vec<Player>,
}

struct Player {
    connection: TcpStream,
    ip: IpAddr,
    name: String,
    id: String,
}

fn main() {
    let listener = match TcpListener::bind(ADDR) {
        Ok(l) => l,
        Err(e) => panic!("{e:?}"),
    };

    let mut handles = Vec::new();
    
    SimpleLogger::new().init().unwrap();

    for stream in listener.incoming() {
        match stream {
            Ok(s) => handles.push(thread::spawn(move || handle_connection(s))),
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

fn handle_connection(mut stream: TcpStream) {
    loop {
        let mut buf: [u8; 1024] = [0; 1024];
        let _ =stream.read(&mut buf);
        if buf[0] == 0 {
            break;
        }
        // println!("{buf:?}");
        
        let length_ivar = ivar::read(&buf).unwrap();
        let length = length_ivar.length();
        println!("Length: {:?}", length);

        // Packet ID matching
        match buf[1] {
            0x00 => {
                handshake(&mut stream, &buf[length..=length_ivar.value as usize]);
            },
            _ => error!("Unrecognised packet"),
        }
    }
}

fn handshake(stream: &mut TcpStream, buffer: &[u8]) {
    
    match stream.local_addr() {
        Ok(addr) => info!("Starting handshake with: {}", addr.ip()),
        Err(e) => {
            error!("Could not get ip from client!\n{e:?}");
            return;
        },
    }

    let packet_id_varint: ivar = ivar::read(&buffer).unwrap();
    let packet_id_length = packet_id_varint.length();
    
    let protocol_varint: ivar = match ivar::read(&buffer[packet_id_length..]) {
        Ok(e) => e,
        Err(e) => {
            error!("{buffer:?}\n{e:?}");
            ivar::new(1000)
        },
    };

    let state: HandshakeState = HandshakeState::from_u8(*buffer.last().unwrap());
    debug!("Protocl: {}, State: {}", protocol_varint.value, state);
    
    match state {
        HandshakeState::Status => {
            status(stream);
        },
        HandshakeState::Login => {},
        HandshakeState::Transfer => {},
        HandshakeState::Unknown => {
            match stream.local_addr() {
                Ok(addr) => info!("Unknown handshake state from {}", addr.ip()),
                Err(e) => error!("Could not get ip from client!\n{e:?}"),
            }
            let _ = stream.shutdown(std::net::Shutdown::Both);
        },
    }
}

fn status(stream: &mut TcpStream) {
    let x = StatusResponse::new();
    let packet_id = ivar::new(0).as_bytes();
    let field_name = "JSON Response".as_bytes();
    let response_string = serde_json::to_string(&x).unwrap();
    let response_length = ivar::new(response_string.len() as i32);
    let mut buffer: Vec<u8> = Vec::new();

    buffer.extend_from_slice(&packet_id);
    buffer.extend_from_slice(&field_name);

    buffer.extend_from_slice(&response_length.as_bytes());
    buffer.extend_from_slice(&response_string.as_bytes());
    let p = stream.write_all(&buffer);
    info!("{p:?}");
}

#[derive(Debug, Serialize)]
enum HandshakeState {
    Status = 1,
    Login = 2,
    Transfer = 3,
    Unknown = 4,
}

impl HandshakeState {
    fn from_u8(value: u8) -> HandshakeState {
        match value {
            1 => Self::Status,
            2 => Self::Login,
            3 => Self::Transfer,
            _ => {
                error!("Unknown handshake state: {}!", value);
                Self::Unknown
            } 
        }
    }
}

impl fmt::Display for HandshakeState {
    fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
        match self {
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
    favicon: &'a str,
    enforces_secure_chat: bool,
}

#[derive(Debug, Serialize)]
struct Version <'a> {
    name: &'a str,
    protocol: usize,
}

#[derive(Debug, Serialize)]
struct DisplayPlayer {}

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
                protocol: 762,
            },
            players: Players {
                max: 100,
                online: 12,
                sample: Vec::new(),
            },
            description: Description {
                text: "OwO",
            },
            favicon: "data:image/png;base64,<data>",
            enforces_secure_chat: false,
        }
    }
}
