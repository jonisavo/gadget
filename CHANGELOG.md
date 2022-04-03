## 0.9.2 [2022-04-03]

### `InlineAttribute` & `SerializeReferenceSelectorAttribute`
- Fix type dropdown being empty if the field is a List<T>

## 0.9.1 [2022-04-03]

- Update documentation

### `GadgetMultilineAttribute`
- Fix additional identation of the text area

## 0.9.0 [2022-04-02]

### `GadgetDrawerExtension`
- Remove `currentHeight` argument from `TryOverrideHeight`
- Add `label` argument of type `GUIContent` to `TryOverrideHeight`

### `GadgetMultilineAttribute`
- Fix erroneous placement of the text area

### `InlineAttribute`
- Fix incorrect height in arrays

### `EnableIfAttribute`, `ShowIfAttribute`, `GadgetContextMenuItemAttribute`
- Fix usage in nested structures

### `InlineAttribute` & `SerializeReferenceSelectorAttribute`
- Display a "nice" version of the type name if possible

### `GadgetContextMenuItem`
- Show error when no method name is specified

### `ShowIfAttribute`
- Show error if the field is a List<>

### `GadgetTooltipAttribute`
- Show error when the given text is null or empty

## 0.8.0 [2022-03-31]

### `ReferenceTypeSelectorAttribute`
- Renamed from `SerializeReferenceSelectorAttribute`

### `InlineAttribute` & `SerializeReferenceSelectorAttribute`
- Fix concrete field type not being selectable in type context menus

## 0.7.0 [2022-03-31]

- Rename `PropertyDrawerExtensionBase` to `GadgetDrawerExtension`
- Remove `PropertyDrawerExtension<T>`
- Add `GadgetExtensionForAttribute` for linking an extension to an attribute

## 0.6.0 [2022-03-30]

- Cache `GUIContent` label and `FieldInfo` in `PropertyDrawerExtensionBase`
- Call `OnPreGUI` and `OnPostGUI` before and after disabled groups, respectively
- Fix error box drawing in array elements

### `ShowIfAttribute`
- No longer has an effect in arrays

### `GadgetTooltipAttribute`
- Added

### `GadgetMultilineAttribute`
- Added

### `GadgetDelayedAttribute`
- Added

### `GadgetMinAttribute`
- Added

### `GadgetGradientUsageAttribute`
- Added

### `GadgetColorUsageAttribute`
- Added

### `GadgetContextMenuItemAttribute`
- Added

### `GadgetRangeAttribute`
- Added

## 0.5.0 [2022-03-27]

- Rename package to `io.savolainen.gadget.core`
- Migrate project under `Gadget` namespace
- Rename `BasePropertyAttribute` to `GadgetPropertyAttribute`
- Rename `BasePropertyDrawer` to `GadgetPropertyDrawer`

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
