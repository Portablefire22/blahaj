use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize)]
pub struct PaintingVariant {
    asset_id: String,
    height: i32,
    width: i32,
}

impl Default for PaintingVariant {
    fn default() -> Self {
        Self {
            asset_id: "minecraft:pigscene".into(),
            height: 4,
            width: 4,
        }
    }
}
