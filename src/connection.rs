use std::{io::Read, net::{IpAddr, Shutdown, TcpStream}};

use log::{error, info, debug};
use serde::Serialize;

use crate::types::varint::{ivar, VarIntDecodeError};

pub struct Connection {
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

    pub fn read_raw(&mut self, buffer: &mut [u8]) -> Result<usize, std::io::Error> {
        self.stream.read(buffer)
    }

    pub fn read(&mut self, buffer: &mut [u8]) -> Result<(Vec<u8>, usize), VarIntDecodeError> {
        let _ = match self.stream.read(buffer) {
            Err(e) => {
                self.shutdown(Shutdown::Both, Some(format!("{e:?}")))
            },
            Ok(_) => Ok(()),
        };
        match ivar::read(buffer) {
            Ok(value) => {
                let buffer: Vec<u8> = Vec::from(&buffer[..=value.value as usize]);
                debug!("b: {buffer:?}");
                Ok((buffer, value.length()))
            },
            Err(e) => {
                error!("VarIntDecodeError whilst reading buffer!: {e:?}");
                Err(e)
            }
        }
    }

    pub fn shutdown(&mut self,how: Shutdown, reason: Option<String>) -> Result<(), std::io::Error>{
        if reason.is_some() {
            info!("Disconnecting {}, Reason: {}", self.ip, reason.unwrap());
        } else {
            info!("Disconnecting {}, reason unspecified!", self.ip);
        } 
        self.stream.shutdown(how)
    }

    pub fn set_state(&mut self, state: ConnectionState) {
        self.state = state;
    }

    pub fn get_state(&self) -> &ConnectionState {
        &self.state
    }

    pub fn get_stream(&mut self) -> &mut TcpStream {
        &mut self.stream
    }

    pub fn ip(&self) -> IpAddr {
        self.ip
    }
}

#[derive(Debug, Serialize)]
pub enum ConnectionState {
    Handshake = 0,
    Status = 1,
    Login = 2,
    Transfer = 3,
    Unknown = 4,
}

impl ConnectionState {
    pub fn from_u8(value: u8) -> ConnectionState {
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
