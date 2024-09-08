use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize)]
pub struct ChatType {
    chat: Decoration,
    narration: Decoration,
}

#[derive(Serialize, Deserialize)]
pub struct Decoration {
    translation_key: String,
    style: Option<String>,
    parameters: Vec<String>,
}

impl Default for ChatType {
    fn default() -> Self {
        Self {
            chat: Decoration {
                style: None,
                parameters: vec!["sender".into(), "content".into()],
                translation_key: "chat.type.text".into(),
            },
            narration: Decoration {
                style: None,
                parameters: vec!["sender".into(), "content".into()],
                translation_key: "chat.type.text.narrate".into(),
            },
        }
    }
}
