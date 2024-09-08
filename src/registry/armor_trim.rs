use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize)]
pub struct ArmorTrimMaterial {
    asset_name: String,
    ingredient: String,
    item_model_index: f32,
    override_armor_materials: Option<String>,
    description: String,
}


#[derive(Serialize, Deserialize)]
pub struct OverrideArmorMaterials {

}


#[derive(Serialize, Deserialize)]
pub struct ArmorTrimPattern {
    asset_id: String,
    template_item: String,
    description: String,
    decal: i8,
}



