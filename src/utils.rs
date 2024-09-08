use crate::types::varint::{ivar, VarIntDecodeError};

pub fn read_packet_id(buffer: &[u8]) -> Result<ivar, VarIntDecodeError> {
    ivar::read(buffer)
}

pub fn write_ivar(buffer: &mut Vec<u8>, id: i32) {
    let mut data = ivar::new(id).as_bytes();
    buffer.append(&mut data);
}

pub fn write_utf8_string(buffer: &mut Vec<u8>, text: String) {
    let mut data: Vec<u8> = text.into_bytes();
    buffer.extend_from_slice(&ivar::new(data.len() as i32).as_bytes());
    buffer.append(&mut data);
}
