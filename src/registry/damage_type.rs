use fastnbt::SerOpts;
use serde::{Deserialize, Serialize};

use crate::registry_data::RegistryEntry;

#[derive(Serialize, Deserialize)]
pub struct DamageType {
    message_id: String,
    scaling: String,
    exhaustion: f32,
    effects: Option<String>,
    death_message_type: Option<String>,
}

const NAMES: &[&str] = &[
    "arrow",
    "bad_respawn_point",
    "cactus",
    "cramming",
    "campfire",
    "cramming",
    "dragon_breath",
    "drown",
    "dry_out",
    "explosion",
    "fall",
    "falling_anvil",
    "falling_block",
    "falling_stalactite",
    "fireball",
    "fireworks",
    "fly_into_wall",
    "freeze",
    "generic",
    "generic_kill",
    "hot_floor",
    "in_fire",
    "in_wall",
    "indirect_magic",
    "lava",
    "lightning_bolt",
    "magic",
    "mob_attack",
    "mob_attack_no_aggro",
    "mob_projectile",
    "on_fire",
    "out_of_world",
    "outside_border",
    "player_attack",
    "player_explosion",
    "sonic_boom",
    "spit",
    "stalagmite",
    "starve",
    "sting",
    "sweet_berry_bush",
    "thorns",
    "thrown",
    "trident",
    "unattributed_fireball",
    "wither",
    "wither_skull",
];

pub fn entries() -> Vec<RegistryEntry> {
    let items: Vec<_> = NAMES
        .iter()
        .map(|name| RegistryEntry {
            entry_id: name.to_string(),
            has_data: true,
            data: Some(fastnbt::to_bytes(
                &DamageType {
                    exhaustion: 0.1,
                    message_id: "inFire".into(),
                    scaling: "when_caused_by_living_non_player".into(),
                    death_message_type: None,
                    effects: None,
                },
            ).unwrap()),
        })
        .collect();

    items
}
