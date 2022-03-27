## 0.4.0 [2022-03-27]

- Create a `PropertyDrawerExtension` system for stacked attributes
- Migrate all existing attributes to use `PropertyDrawerExtension`
- All attributes can be used simultaneously (except `InlineAttribute` and `SerializeReferenceSelectorAttribute`)

### `DisabledAttribute`
- Added

### `ShowIfAttribute` & `EnableIfAttribute`
- Add support for non-serialized fields

## 0.3.0 [2022-03-26]

### `ShowIfAttribute` & `EnableIfAttribute`
- Add support for properties and methods

### `ShowIfAttribute`
- Fix error box not being drawn correctly if the attribute is given an invalid member name

## 0.2.1 [2022-03-25]

- Document the public API

### `InlineAttribute`
- Fix InvalidOperationException after selecting asset

### `InlineAttribute` & `SerializeReferenceSelectorAttribute`
- Fix structs not showing up in type lists

## 0.2.0 [2022-03-24]
### `InlineAttribute`
- Show inline creation on supported types by default
- Replace `AllowInlineCreation` property with `DisallowInlineCreation` for the opposite effect
- Add initial support for inline creation of Materials
- Remove assembly name from the shown type name

### `InlineAttribute` & `SerializeReferenceSelectorAttribute`
- If only one type option exists, select it without showing a menu
- Do not show sealed types as options
- Do not show types marked with `[Obsolete]` as options
- Fix type list headers breaking when the property field type is an array
- Add error message on invalid fields

### `TypeMenuPathAttribute`
- Renamed from `TypeMenuNameAttribute`

## 0.1.0 [2022-03-24]
- Initial in-development version
