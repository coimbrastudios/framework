# [Coimbra Framework](Index.md): Scriptable Settings

Easily access a [ScriptableObject] from anywhere with option to preload those on the application startup.

[ScriptableSettings] are singleton-like objects meant to be used as globally-accessible read-only data containers.
Their creation usually happens inside the editor and are persistent at runtime.

> Most of those things aren't enforced anywhere, but it is strongly encouraged to only use [ScriptableSettings] for this use case to avoid code-smells and bad design decisions.

You can check its usage details in the [ScriptableSettings] APIs and in `Difficulty Settings` package sample.

## Implementing Settings

To implement a new settings you only need to:

1. Inherit from [ScriptableSettings].
2. Add you data fields (usually private fields with public getters).
3. Add either [CreateAssetMenuAttribute], [PreferencesAttribute], or [ProjectSettingsAttribute].
4. (Optional) Enable the `Preload` option in the inspector to add it to the `Preloaded Assets`.

> `Preload` will always be true for any [ScriptableSettings] with [ProjectSettingsAttribute] that isn't editor-only.
> Also, it will always be false for any editor-only [ScriptableSettings], including ones with [PreferencesAttribute].

## Implementing Editors

When exposed to Preferences or Project Settings the [ScriptableSettings] provides built-in support for the search functionality.
To easily preserve the search support with your own [Custom Editor] you can inherit from [ScriptableSettingsEditor] and use the provided APIs.

[PreferencesAttribute]:<../Coimbra/PreferencesAttribute.cs>

[ProjectSettingsAttribute]:<../Coimbra/ProjectSettingsAttribute.cs>

[ScriptableSettings]:<../Coimbra/ScriptableSettings.cs>

[ScriptableSettingsEditor]:<../Coimbra.Editor/ScriptableSettingsEditor.cs>

[CreateAssetMenuAttribute]:<https://docs.unity3d.com/ScriptReference/CreateAssetMenuAttribute.html>

[ScriptableObject]:<https://docs.unity3d.com/ScriptReference/ScriptableObject.html>

[Custom Editor]:<https://docs.unity3d.com/Manual/editor-CustomEditors.html>
