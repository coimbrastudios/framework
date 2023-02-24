# [Coimbra Framework](Index.md): Serialization Helpers

A few custom [PropertyAttribute] to change the display of your properties without requiring custom editors.

## Custom Types

There are some custom types provided to help with common serialization limitations:

- [AssetReferenceScene](../Coimbra/AssetReferenceScene.cs): strong-reference to scenes without unsafe workarounds.
- [Delayed](../Coimbra/Delayed`1.cs): wrapper to add the [DelayedAttribute] to a generic type argument.
- [DelegateListener](../Coimbra/DelegateListener.cs): allows inspection of any delegate listener, but without actual serialization support.
- [FloatRange](../Coimbra/FloatRange.cs): similar to a [Vector2] for when you just need a min and max value.
- [IntRange](../Coimbra/IntRange.cs): similar to a [Vector2Int] for when you just need a min and max value.
- [ManagedField]: expose any interface, without losing support to [Object], or any plain C# class that respects the [SerializeReference] restrictions and has a parameterless constructor.
- [Reference]: wrapper for any value that is meant to be treated as a reference in code, but its serialization remains value-based.
- [SerializableDictionary]: serializable version of [Dictionary] that also supports nested collections directly.
- [SerializableType]: serialize any [Type] object.
- [SerializableTypeDictionary]: serializable version of [Dictionary] which the key is always a [SerializableType] and the value is a [ManagedField] holding an instance of the same type as the key.

## Decorator Attributes

There are some [PropertyAttribute] with custom [DecoratorDrawer](https://docs.unity3d.com/ScriptReference/DecoratorDrawer.html) implementations which should be pretty self-explanatory:

- [Disable](../Coimbra/PropertyAttributes/DisableAttribute.cs)
- [DisableOnEditMode](../Coimbra/PropertyAttributes/DisableOnEditModeAttribute.cs)
- [DisableOnPlayMode](../Coimbra/PropertyAttributes/DisableOnPlayModeAttribute.cs)
- [MessageBox](../Coimbra/PropertyAttributes/MessageBoxAttribute.cs)
- [MessageBoxOnEditMode](../Coimbra/PropertyAttributes/MessageBoxOnEditModeAttribute.cs)
- [MessageBoxOnPlayMode](../Coimbra/PropertyAttributes/MessageBoxOnPlayModeAttribute.cs)
- [Indent](../Coimbra/PropertyAttributes/IndentAttribute.cs)

## Property Attributes

There are some [PropertyAttribute] with custom [PropertyDrawer](https://docs.unity3d.com/ScriptReference/PropertyDrawer.html) implementations:

| [PropertyAttribute]                                                              | Field Types                                                                                  |
|----------------------------------------------------------------------------------|----------------------------------------------------------------------------------------------|
| [AnimatorParameter](../Coimbra/PropertyAttributes/AnimatorParameterAttribute.cs) | `string`                                                                                     |
| [AssetsOnly](../Coimbra/PropertyAttributes/AssetsOnlyAttribute.cs)               | [Object]                                                                                     |
| [EnumFlags](../Coimbra/PropertyAttributes/EnumFlagsAttribute.cs)                 | `enum`                                                                                       |
| [FloatRange](../Coimbra/PropertyAttributes/FloatRangeAttribute.cs)               | [FloatRange], [Vector2]                                                                      |
| [IntRange](../Coimbra/PropertyAttributes/IntRangeAttribute.cs)                   | [IntRange], [Vector2Int]                                                                     |
| [LayerSelector](../Coimbra/PropertyAttributes/LayerSelectorAttribute.cs)         | `int` `string`                                                                               |
| [NotGreaterThan](../Coimbra/PropertyAttributes/NotGreaterThanAttribute.cs)       | `int` `float`                                                                                |
| [NotLessThan](../Coimbra/PropertyAttributes/NotLessThanAttribute.cs)             | `int` `float`                                                                                |
| [RangeSlider](../Coimbra/PropertyAttributes/RangeSliderAttribute.cs)             | [FloatRange], [IntRange], [Vector2], [Vector2Int]                                            |
| [SelectableLabel](../Coimbra/PropertyAttributes/SelectableLabelAttribute.cs)     | `string`                                                                                     |
| [SortingLayerID](../Coimbra/PropertyAttributes/SortingLayerIDAttribute.cs)       | `int`                                                                                        |
| [TagSelector](../Coimbra/PropertyAttributes/TagSelectorAttribute.cs)             | `string`                                                                                     |
| [TypeDropdown](../Coimbra/PropertyAttributes/TypeDropdownAttribute.cs)           | Both follows the [SerializeReference] restrictions and contains a parameterless constructor. |
| [Validate](../Coimbra/PropertyAttributes/ValidateAttribute.cs)                   | See [Script Serialization] restrictions                                                      |

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

- [By Accessibility](../Coimbra/FilterTypesAttributes/FilterTypesByAccessibilityAttribute.cs)
- [By Assignable From](../Coimbra/FilterTypesAttributes/FilterTypesByAssignableFromAttribute.cs)
- [By Method](../Coimbra/FilterTypesAttributes/FilterTypesByMethodAttribute.cs)
- [By Specific Type](../Coimbra/FilterTypesAttributes/FilterTypesBySpecificTypeAttribute.cs)

## Other Attributes

There are other attributes included in the package to help customize the in-editor experience:

- [AssetReferenceComponentRestriction](../Coimbra/AssetReferenceComponentRestriction.cs): filters an asset reference to require a given component when being assigned in the inspector.
- [DisablePickerAttribute](../Coimbra/DisablePickerAttribute.cs): apply to a [ManagedField] to disallow selecting another instance in the inspector while still allowing to edit the current instance serialized values.
- [DisableResizeAttribute](../Coimbra/DisableResizeAttribute.cs): apply to a [SerializableDictionary] or [SerializableTypeDictionary] to disable its resizing in the inspector.
- [FormerlySerializedAsBackingFieldOfAttribute](../Coimbra/FormerlySerializedAsBackingFieldOfAttribute.cs): apply to a field that was previously a property to not lose its serialized value.
- [HideKeyLabelAttribute](../Coimbra/HideKeyLabelAttribute.cs): apply to a [SerializableDictionary] or [SerializableTypeDictionary] to hide the key label in the inspector.
- [HideValueLabelAttribute](../Coimbra/HideValueLabelAttribute.cs): apply to a [SerializableDictionary] or [SerializableTypeDictionary] to hide the value label in the inspector.


## [PropertyPathInfo]

Reflection helper class for any type following the [Script Serialization] restriction based on its [propertyPath](https://docs.unity3d.com/ScriptReference/SerializedProperty-propertyPath.html).

You can check its API documentation for further usage details.

[FloatRange]:<../Coimbra/FloatRange.cs>

[IntRange]:<../Coimbra/IntRange.cs>

[ManagedField]:<../Coimbra/ManagedField`1.cs>

[PropertyPathInfo]:<../Coimbra/PropertyPathInfo.cs>

[Reference]:<../Coimbra/Reference`1.cs>

[SerializableDictionary]:<../Coimbra/SerializableDictionary`2.cs>

[SerializableType]:<../Coimbra/SerializableType`1.cs>

[SerializableTypeDictionary]:<../Coimbra/SerializableTypeDictionary`3.cs>

[DelayedAttribute]:<https://docs.unity3d.com/ScriptReference/DelayedAttribute.html>

[Object]:<https://docs.unity3d.com/ScriptReference/Object.html>

[PropertyAttribute]:<https://docs.unity3d.com/ScriptReference/PropertyAttribute.html>

[PropertyDrawer]:<https://docs.unity3d.com/ScriptReference/PropertyDrawer.html>

[Script Serialization]:<https://docs.unity.cn/Documentation/Manual/script-Serialization.html>

[SerializeField]:<https://docs.unity3d.com/ScriptReference/SerializeField.html>

[SerializeReference]:<https://docs.unity3d.com/ScriptReference/SerializeReference.html>

[Vector2]:<https://docs.unity3d.com/ScriptReference/Vector2.html>

[Vector2Int]:<https://docs.unity3d.com/ScriptReference/Vector2Int.html>

[Dictionary]:<https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2>

[Type]:<https://learn.microsoft.com/en-us/dotnet/api/system.type>
