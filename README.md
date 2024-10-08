# Sera

Format-agnostic (de)serialization abstraction layer

Inspired by [serde-rs](https://github.com/serde-rs/serde)

**Current status: Waiting for new features of C#, current C# abstraction capability is insufficient**

## In progress

- [ ] Core
  - [x] Basic abstract interface
  - [ ] Runtime emit implementation
    - [x] Serialize emit
    - [ ] Deserialize emit
  - [ ] Source generator implementation

- [ ] Planned official implementation formats
  - [ ] Json
    - [x] Serialize
    - [ ] Deserialize
  - [ ] Toml
  - [ ] Yaml
  - [ ] Xml
  - [ ] Msgpack
  - [ ] Cbor
  - [ ] Bson
