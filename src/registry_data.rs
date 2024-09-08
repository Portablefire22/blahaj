use std::{io::Write, net::TcpStream};

use log::debug;

use crate::{convert_buf_to_string, types::varint::ivar, utils::write_utf8_string};


pub fn send_registry_packet(stream: &mut TcpStream, buf: &[u8]) -> Result<(), std::io::Error> { 
    stream.write_all(buf)
}

pub fn construct_registry_packet(registry_id: String, entries: Vec<RegistryEntry>) -> Vec<u8> {
    let mut buffer: Vec<u8> = Vec::new();
    buffer.append(&mut ivar::new(0x07).as_bytes());
    write_utf8_string(&mut buffer, registry_id); 
    buffer.append(&mut ivar::new(entries.len() as i32).as_bytes());

    entries.iter().for_each(|entry| {
        buffer.append(&mut entry.as_bytes())
    });
    let length_bytes = ivar::new(buffer.len() as i32).as_bytes();
    let mut end_buffer: Vec<u8> = Vec::new();
    end_buffer.extend_from_slice(&length_bytes);
    end_buffer.append(&mut buffer);
    debug!("by: {:?} \n {}", end_buffer, convert_buf_to_string(&end_buffer));
    end_buffer
}

pub struct RegistryEntry {
    pub entry_id: String,
    pub has_data: bool,
    pub data: Option<Vec<u8>>,
}

impl RegistryEntry {
    fn as_bytes(&self) -> Vec<u8> {
        let mut buff: Vec<u8> = Vec::new();
    
        write_utf8_string(&mut buff, self.entry_id.clone());

        buff.push(match self.has_data {
            true => 0x1,
            false => 0x0,
        });
        match &self.data {
            Some(data) => {
                buff.extend_from_slice(data);  
            },
            None => (),
        }
        buff
    }
}
