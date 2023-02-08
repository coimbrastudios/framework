# Coimbra Framework

## List of Content

- [Actor](Actor.md): Pretend you are inheriting from [GameObject] to make easier to design per-object behaviours. You probably have been doing that already, but without proper built-in support.
- [Assembly Definition Rules](AssemblyDefinitionRules.md): Linting tools for assembly definition assets. It comes with a few useful implementations and offers support to create custom ones easily.
- [Editor Tools](EditorTools.md): This package offers some tools available in `Tools/Coimbra Framework/` and an option to configure a startup scene while playing in the editor.
- [Event Service](EventService.md): Strongly-typed event system that takes full advantage of `Source Generators` and `Roslyn Analyzers`. Made to be as performant and easy to use as possible.
- [Pooling](Pooling.md): Offers a thread-safe solution for pooling managed object and an [Addressables]-compatible solution for pooling [GameObject]. Should have no conflict with `Unity` pooling APIs.
- [Property Attributes](PropertyAttributes.md): A few custom [PropertyAttribute] to change the display of your properties without requiring custom editors. Also contains helpers to make easier to build a custom [PropertyDrawer].
- [Roslyn Analyzers](RoslynAnalyzers.md): Some analyzers are provided to guide how to use this framework correctly. Check the documentation for the complete list of available analyzers.
- [Service Locator](ServiceLocator.md): Alternative way to design your code without needing the singleton pattern. Comes with support for `Enter Play Mode Options` and a nice debug window.
- [Scriptable Settings](ScriptableSettings.md): Easily access a [ScriptableObject] from anywhere with option to preload those on the application startup. They can also be easily exposed to `Project Settings` and `Preferences`.
- GUI Scopes:
    - [BackgroundColorScope](../Coimbra.Editor/GUIScopes/BackgroundColorScope.cs)
    - [HierarchyModeScope](../Coimbra.Editor/GUIScopes/HierarchyModeScope.cs)
    - [LabelWidthScope](../Coimbra.Editor/GUIScopes/LabelWidthScope.cs)
    - [ResetIndentLevelScope](../Coimbra.Editor/GUIScopes/ResetIndentLevelScope.cs)
    - [ShowMixedValueScope](../Coimbra.Editor/GUIScopes/ShowMixedValueScope.cs)
- Managed Jobs:
    - [IManagedJob](../Coimbra.Jobs/IManagedJob.cs)
    - [IManagedJobParallelFor](../Coimbra.Jobs/IManagedJobParallelFor.cs)
- Scene Processing:
    - [ISceneProcessorComponent](../Coimbra/ISceneProcessorComponent.cs)
    - [IScenePostProcessorComponent](../Coimbra/IScenePostProcessorComponent.cs)
- Serializable Types:
    - [AssetReferenceScene](../Coimbra/AssetReferenceScene.cs)
    - [FloatRange](../Coimbra/FloatRange.cs)
    - [IntRange](../Coimbra/IntRange.cs)
    - [ManagedField](../Coimbra/ManagedField`1.cs)
    - [Reference](../Coimbra/Reference`1.cs)
    - [SerializableType](../Coimbra/SerializableType`1.cs)
    - [SerializableTypeDictionary](../Coimbra/SerializableTypeDictionary`3.cs)
- Useful Attributes:
    - [AssetReferenceComponentRestriction](../Coimbra/AssetReferenceComponentRestriction.cs)
    - [CopyBaseConstructorsAttribute](../Coimbra/CopyBaseConstructorsAttribute.cs)
    - [FormerlySerializedAsBackingFieldOfAttribute](../Coimbra/FormerlySerializedAsBackingFieldOfAttribute.cs)
- And many utility classes with some extension methods.

[Addressables]:<https://docs.unity3d.com/Manual/com.unity.addressables.html>
[GameObject]:<https://docs.unity3d.com/ScriptReference/GameObject.html>
[PropertyAttribute]:<https://docs.unity3d.com/ScriptReference/PropertyAttribute.html>
[PropertyDrawer]:<https://docs.unity3d.com/ScriptReference/PropertyDrawer.html>
[ScriptableObject]:<https://docs.unity3d.com/ScriptReference/ScriptableObject.html>
