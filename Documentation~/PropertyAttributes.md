# [Coimbra Framework](Index.md): Property Attributes

A few custom [PropertyAttribute] to change the display of your properties without requiring custom editors.

## Decorator Attributes

- [DisableAttributes](../Coimbra/PropertyAttributes/DisableAttribute.cs)
- [MessageBoxAttributes](../Coimbra/PropertyAttributes/MessageBoxAttribute.cs)
- [IndentAttribute](../Coimbra/PropertyAttributes/IndentAttribute.cs)

## Property Attributes

- `AnimatorParameter`: turns a string field into a parameter selector for a given animator.
- `AssetsOnly`: prevents to assign a scene object to a `UnityEngine.Object` field.
- `EnumFlags`: turns an enum field into a enum mask popup field.
- `IntRange`: draws a property as if it was a `IntRange`.
- `FloatRange`: draws a property as if it was a `FloatRange`.
- `LayerSelector`: turns an int field into a layer popup field.
- `NotGreaterThan`: prevents an int field to have a value greater than the a given value.
- `NotLessThan`: prevents an int field to have a value smaller than the a given value.
- `RangeSlider`: draws a property using the Unity's `MinMaxSlider`.
- `SelectableLabel`: turns a string field into a selectable label.
- `SortingLayerID`: turns an int field into a sorting layer popup field.
- `TagSelector`: turns a string field into a tag popup field.
- `TypeDropdown`: use in combination with `SerializeReferenceAttribute` to expose a type selector. Can also be combined with `FilterTypesAttributeBase`.
- `Validate`: calls a method `void()` or `void(T previous)` when the property is changed. It is also the base for all others attributes.

## [PropertyPathInfo]

Reflection helper class for any `SerializeField` based on its [propertyPath](https://docs.unity3d.com/ScriptReference/SerializedProperty-propertyPath.html).

[PropertyPathInfo]:<../Coimbra/PropertyPathInfo.cs>

[PropertyAttribute]:<https://docs.unity3d.com/ScriptReference/PropertyAttribute.html>

[PropertyDrawer]:<https://docs.unity3d.com/ScriptReference/PropertyDrawer.html>

