use std::default;

use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize)]
pub struct DimensionType {
    fixed_time: Option<i64>,
    has_skylight: i8,
    has_ceiling: i8,
    ultrawarm: i8,
    natural: i8,
    coordinate_scale: f64,
    bed_works: i8,
    respawn_anchor_works: i8,
    min_y: i32,
    height: i32,
    logical_height: i32,
    infiniburn: String,
    effects: DimensionEffect,
    ambient_light: f32,
    piglin_safe: i8,
    has_raids: i8,
    monster_spawn_light_level: i32,
    monster_spawn_block_light_limit: i32,
}

#[derive(Serialize, Deserialize, Default)]
pub enum DimensionEffect {
    #[serde(rename = "minecraft:overworld")]
    #[default]
    Overworld,
    #[serde(rename = "minecraft:the_nether")]
    TheNether,
    #[serde(rename = "minecraft:the_end")]
    TheEnd,
}

impl Default for DimensionType {
    fn default() -> Self {
       Self {
            fixed_time: None,
            has_skylight: 1,
            has_ceiling: 0,
            ultrawarm: 0,
            natural: 1,
            coordinate_scale: 1.0,
            bed_works: 1,
            respawn_anchor_works: 0,
            min_y: -64,
            height: 384,
            logical_height: 384,
            infiniburn: "minecraft:infiniburn_overworld".into(),
            effects: DimensionEffect::default(),
            ambient_light: 0.0,
            piglin_safe: 0,
            has_raids: 1,
            monster_spawn_light_level: 7,
            monster_spawn_block_light_limit: 15,
        }
    }
}
