use std::{error::Error, isize};
use log::error;
use serde::{Deserializer, Serializer};

#[derive(Debug)]
pub struct VarInt {
    pub value: i32,
    pub bytes: Vec<u8>,
}

pub type ivar = VarInt;

const SEGMENT_BITS: i32 = 0x7F;
const CONTINUE_BIT: i32 = 0x80;

impl VarInt {
    // Max number of bytes that a VarInt can be when read or written to Minecraft
    const MAX_SIZE: usize = 5;

    pub fn new(value: i32) -> Self {
        let mut x = Self {
            value,
            bytes: Vec::new(),
        };
        x.bytes = x.as_bytes();
        x
    }
    
    /// How many bytes the variable int takes up
    pub fn length(&self) -> usize {
        self.bytes.len()
    }

    pub fn read(bytes: &[u8]) -> Result<Self, VarIntDecodeError> {
        let mut val = 0;
        for i in 0..Self::MAX_SIZE {
            let byte = match bytes.get(i) {
                Some(b) => b,
                None => {
                    error!("VarInt decode out of range!");
                    return Err(VarIntDecodeError::OutOfRange);
                }
            };
            val |= (i32::from(*byte) & 0b01111111) << (i * 7);
            if byte & 0b10000000 == 0 {
                return Ok(VarInt::new(val));
            }
        }
        Err(VarIntDecodeError::TooLarge)
    }
    
    pub fn as_bytes(&self) -> Vec<u8> {
        let mut value = self.value as u64;
        let mut bytes: Vec<u8> = Vec::new();
        loop {
            let byte = (value & 0x7f) as u8;
            value >>= 7;

            if value == 0 {
                bytes.push(byte);
                break;
            }
            bytes.push(byte | 0x80);
        }
        bytes
    }
}

impl<'de> serde::Deserialize<'de> for ivar {
    fn deserialize<D: Deserializer<'de>>(d: D) -> Result<Self, D::Error> {
        let s = i32::deserialize(d)?;
        Ok(ivar::new(s))
    }
}
impl serde::Serialize for ivar {
    fn serialize<S>(&self, serializer: S) -> Result<S::Ok, S::Error> 
        where S: Serializer,
    {
        serializer.serialize_i32(self.value)
    }
}



#[derive(Debug)]
pub enum VarIntDecodeError {
    Incomplete,
    TooLarge,
    OutOfRange,
}

#[derive(Debug)]
pub enum VarIntEncodeError {
    Incomplete,
    TooLarge,
    OutOfRange,
}
