use serde::{Deserialize, Serialize};

use crate::types::varint::ivar;
#[derive(Serialize, Deserialize)]
pub struct BannerPattern {
    asset_id: String,
    translation_key: String,
}
