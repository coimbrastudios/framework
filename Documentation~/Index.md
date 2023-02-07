# Coimbra Framework

## List of Content

- [Actor](Actor.md): Pretend you are inheriting from `GameObject` to make easier to design per-object behaviours. You probably have been doing that already, but without proper built-in support.
- [Assembly Definition Rules](AssemblyDefinitionRules.md): linting tools for assembly definition assets. It comes with a few useful implementations and support to create custom ones easily.
- [Event Service](EventService.md): Strongly-typed event system that takes full advantage of `Source Generators` and `Roslyn Analyzers`. Made to be as performance and easy to use as possible.
- [Pooling](Pooling.md): Offers a thread-safe solution for pooling managed object and an `Addressables`-compatible solution for pooling `GameObject`.
- [Property Attributes](PropertyAttributes.md): A few custom `PropertyAttribute` to change the display of your properties without requiring custom editors. Also contains helpers to make easier build a custom `PropertyDrawer`.
- [Roslyn Analyzers](RoslynAnalyzers.md): Some analyzers are provided to guide how to use this framework correctly. Check the documentation for the complete list of available analyzers.
- [Service Locator](ServiceLocator.md): Alternative way to design your code without needing the singleton pattern. Comes with support for `Enter Play Mode Options` and a nice debug window.
- [Scriptable Settings](ScriptableSettings.md): Easily access a `ScriptableObject` from anywhere with option to preload those on the application startup. They can also be easily exposed to `Project Settings` and `Preferences`.
- Editor Tools in `Tools/Coimbra Framework/`:
    - `Assert Serializable Types`: Asserts that all types that inherits from a serializable type also contains the `SerializableAttribute`. It will also log a message if everything is correct.
    - `Clear Cache`: Deletes all `AssetBundle` and `ProceduralMaterial` content that has been cached.
    - `Create Assets Assembly`: Generate assemblies for all scripts in the `Assets` folder while also taking into consideration `Editor` folders. Needs to be triggered everytime a new third-party is imported.
    - `Force Reserialize Assets`: Forcibly load and re-serialize all assets, flushing any outstanding data changes to disk. Useful after upgrading engine versions to avoid unnecessary dirtying later on.
    - `Request Script Reload`: The Unity Editor reloads script assemblies asynchronously on the next frame. This resets the state of all the scripts, but Unity does not compile any code that has changed since the previous compilation.
    - `Reset Play Mode Start Scene`: Reset the `EditorSceneManager.playModeStartScene` back to null, if set.
    - `Save Assets`: Saves all assets changes to disk. About the same as `File/Save Project` but also with a nice default shortcut at `Alt + Shift + S`.
- Managed Jobs:
    - [IManagedJob](../Coimbra.Jobs/IManagedJob.cs)
    - [IManagedJobParallelFor](../Coimbra.Jobs/IManagedJobParallelFor.cs)
- Scene Processing:
    - [ISceneProcessorComponent](../Coimbra/ISceneProcessorComponent.cs)
    - [IScenePostProcessorComponent](../Coimbra/IScenePostProcessorComponent.cs)
- Serialization-Friendly Types:
    - [AssetReferenceScene](../Coimbra/AssetReferenceScene.cs)
    - [FloatRange](../Coimbra/FloatRange.cs)
    - [IntRange](../Coimbra/IntRange.cs)
    - [ManagedField](../Coimbra/ManagedField`1.cs)
    - [Reference](../Coimbra/Reference`1.cs)
    - [SerializableType](../Coimbra/SerializableType`1.cs)
    - [SerializableTypeDictionary](../Coimbra/SerializableTypeDictionary`3.cs)
- Useful Attributes.
    - [AssetReferenceComponentRestriction](../Coimbra/AssetReferenceComponentRestriction.cs)
    - [CopyBaseConstructorsAttribute](../Coimbra/CopyBaseConstructorsAttribute.cs)
    - [FormerlySerializedAsBackingFieldOfAttribute](../Coimbra/FormerlySerializedAsBackingFieldOfAttribute.cs)
- And many utility classes with extension methods.
