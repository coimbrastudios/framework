# [Coimbra Framework](Index.md): Property Attributes

A few custom [PropertyAttribute] to change the display of your properties without requiring custom editors.

## Decorator Attributes

There are some [PropertyAttribute] with custom [DecoratorDrawer](https://docs.unity3d.com/ScriptReference/DecoratorDrawer.html) implementations:

- [Disable](../Coimbra/PropertyAttributes/DisableAttribute.cs)
- [DisableOnEditMode](../Coimbra/PropertyAttributes/DisableOnEditModeAttribute.cs)
- [DisableOnPlayMode](../Coimbra/PropertyAttributes/DisableOnPlayModeAttribute.cs)
- [MessageBox](../Coimbra/PropertyAttributes/MessageBoxAttribute.cs)
- [MessageBoxOnEditMode](../Coimbra/PropertyAttributes/MessageBoxOnEditModeAttribute.cs)
- [MessageBoxOnPlayMode](../Coimbra/PropertyAttributes/MessageBoxOnPlayModeAttribute.cs)
- [Indent](../Coimbra/PropertyAttributes/IndentAttribute.cs)

## Property Attributes

There are some [PropertyAttribute] with custom [PropertyDrawer](https://docs.unity3d.com/ScriptReference/PropertyDrawer.html) implementations:

| [PropertyAttribute]                                                              | Field Types                                                                                |
|----------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------|
| [AnimatorParameter](../Coimbra/PropertyAttributes/AnimatorParameterAttribute.cs) | `string`                                                                                   |
| [AssetsOnly](../Coimbra/PropertyAttributes/AssetsOnlyAttribute.cs)               | [Object]                                                                                   |
| [EnumFlags](../Coimbra/PropertyAttributes/EnumFlagsAttribute.cs)                 | `enum`                                                                                     |
| [FloatRange](../Coimbra/PropertyAttributes/FloatRangeAttribute.cs)               | [FloatRange], [Vector2]                                                                    |
| [IntRange](../Coimbra/PropertyAttributes/IntRangeAttribute.cs)                   | [IntRange], [Vector2Int]                                                                   |
| [LayerSelector](../Coimbra/PropertyAttributes/LayerSelectorAttribute.cs)         | `int` `string`                                                                             |
| [NotGreaterThan](../Coimbra/PropertyAttributes/NotGreaterThanAttribute.cs)       | `int` `float`                                                                              |
| [NotLessThan](../Coimbra/PropertyAttributes/NotLessThanAttribute.cs)             | `int` `float`                                                                              |
| [RangeSlider](../Coimbra/PropertyAttributes/RangeSliderAttribute.cs)             | [FloatRange], [IntRange], [Vector2], [Vector2Int]                                          |
| [SelectableLabel](../Coimbra/PropertyAttributes/SelectableLabelAttribute.cs)     | `string`                                                                                   |
| [SortingLayerID](../Coimbra/PropertyAttributes/SortingLayerIDAttribute.cs)       | `int`                                                                                      |
| [TagSelector](../Coimbra/PropertyAttributes/TagSelectorAttribute.cs)             | `string`                                                                                   |
| [TypeDropdown](../Coimbra/PropertyAttributes/TypeDropdownAttribute.cs)           | According the [SerializeReference] restrictions. Requires a parameterless constructor too. |
| [Validate](../Coimbra/PropertyAttributes/ValidateAttribute.cs)                   | According the [Script Serialization] restrictions                                          |

## Filter Type Attributes

Those are special attributes compatible with the following fields:

- [ManagedField]
- [SerializableType]
- [Reference]<[ManagedField]>
- [Reference]<[SerializableType]>
- Fields with [TypeDropdown](../Coimbra/PropertyAttributes/TypeDropdownAttribute.cs)
- [SerializableTypeDictionary] as the last generic type argument

To implement a new filter type attribute you only need to inherit from [FilterTypesAttributeBase](../Coimbra/FilterTypesAttributeBase.cs) and implement its abstract `Validate` method.
There are some default implementations provided so that you can start filter types:

- [By Accessibility](../Coimbra/FilterTypesByAccessibilityAttribute.cs)
- [By Assignable From](../Coimbra/FilterTypesByAssignableFromAttribute.cs)
- [By Method](../Coimbra/FilterTypesByMethodAttribute.cs)
- [By Specific Type](../Coimbra/FilterTypesBySpecificTypeAttribute.cs)

## [PropertyPathInfo]

Reflection helper class for any type following the [Script Serialization] restriction based on its [propertyPath](https://docs.unity3d.com/ScriptReference/SerializedProperty-propertyPath.html).

You can check its API documentation for further usage details.

[FloatRange]:<../Coimbra/FloatRange.cs>

[IntRange]:<../Coimbra/IntRange.cs>

[ManagedField]:<../Coimbra/ManagedField`1.cs>

[PropertyPathInfo]:<../Coimbra/PropertyPathInfo.cs>

[Reference]:<../Coimbra/Reference`1.cs>

[SerializableType]:<../Coimbra/SerializableType`1.cs>

[SerializableTypeDictionary]:<../Coimbra/SerializableTypeDictionary`3.cs>

[Object]:<https://docs.unity3d.com/ScriptReference/Object.html>

[PropertyAttribute]:<https://docs.unity3d.com/ScriptReference/PropertyAttribute.html>

[PropertyDrawer]:<https://docs.unity3d.com/ScriptReference/PropertyDrawer.html>

[Script Serialization]:<https://docs.unity.cn/Documentation/Manual/script-Serialization.html>

[SerializeField]:<https://docs.unity3d.com/ScriptReference/SerializeField.html>

[SerializeReference]:<https://docs.unity3d.com/ScriptReference/SerializeReference.html>

[Vector2]:<https://docs.unity3d.com/ScriptReference/Vector2.html>

[Vector2Int]:<https://docs.unity3d.com/ScriptReference/Vector2Int.html>
