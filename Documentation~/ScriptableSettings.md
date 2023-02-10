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
4. Configure it in the editor.
5. (Optional) If it is not an editor-only [ScriptableSettings] you can enable the `Preload` option in the inspector to add it to the `Preloaded Assets`.

[PreferencesAttribute]:<../Coimbra/PreferencesAttribute.cs>

[ProjectSettingsAttribute]:<../Coimbra/ProjectSettingsAttribute.cs>

[ScriptableSettings]:<../Coimbra/ScriptableSettings.cs>

[CreateAssetMenuAttribute]:<https://docs.unity3d.com/ScriptReference/CreateAssetMenuAttribute.html>

[ScriptableObject]:<https://docs.unity3d.com/ScriptReference/ScriptableObject.html>
