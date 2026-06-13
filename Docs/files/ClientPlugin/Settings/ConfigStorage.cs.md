# `ClientPlugin/Settings/ConfigStorage.cs`

*Serialises and deserialises `Config.Current` as an XML file in the user's Space Engineers storage folder.*

|  |  |
| --- | --- |
| **Module** | [Client Settings UI Framework](../../../modules/client-settings.md) |
| **Source** | [`ConfigStorage.cs`](../../../../ClientPlugin/Settings/ConfigStorage.cs) (44 lines) |
| **Kind** | `Static class` |
| **Role** | Configuration persistence |

## Purpose

`ConfigStorage` owns the round-trip between the in-memory [`Config.cs`](../Config.cs.md) instance and the on-disk `Performance.cfg` file. The file is placed in `<UserDataPath>/Storage/Performance.cfg` using `MyFileSystem.UserDataPath`, so it lives alongside other Space Engineers user data.

`Load` is called once by `Config.Current`'s field initialiser: it deserialises the XML file if it exists and returns the result, or falls back to `Config.Default` when the file is absent or corrupt (logging a warning on parse failure). `Save` is called by [`SettingsScreen.cs`](SettingsScreen.cs.md)'s `OnRemoved` hook whenever the user closes the dialog, ensuring any changes are persisted immediately.

Both methods use `XmlSerializer(typeof(Config))` directly; no custom serialisation attributes are needed because the `Config` properties are plain `bool` values with public getters and setters.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `ConfigFileName` | `private static string` | `"Performance.cfg"` — derived from `Plugin.Name`. |
| `ConfigFilePath` | `private static string` property | Full path: `<UserDataPath>/Storage/Performance.cfg`. |
| `Save(Config)` | `static method` | Serialises `config` to XML at `ConfigFilePath`, creating the directory if needed. |
| `Load()` | `static method` | Deserialises `ConfigFilePath` and returns the result, or `Config.Default` on failure. |

## References

- [`Config.cs`](../Config.cs.md) — the type serialised / deserialised
- [`SettingsScreen.cs`](SettingsScreen.cs.md) — calls `Save` from `OnRemoved`

---

*[Handbook](../../../TOC.md) · [Module: Client Settings UI Framework](../../../modules/client-settings.md) · [Index](../../../Index.md)*
