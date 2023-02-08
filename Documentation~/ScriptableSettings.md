# [Coimbra Framework](Index.md): Scriptable Settings

Easily access a [ScriptableObject] from anywhere with option to preload those on the application startup.

[ScriptableSettings] are singleton-like objects meant to be used as globally-accessible read-only data containers. Their creation usually happens inside the editor and are persistent at runtime.

> Most of those things aren't enforced anywhere, but it is strongly encouraged to only use [ScriptableSettings] for this use case to avoid code-smells and bad design decisions.

You can check its usage details in the [ScriptableSettings] APIs and in `Difficulty Settings` package sample.

## Implementing Settings

To implement a new settings you only need to:

- Inherit from [ScriptableSettings].
- Add you data fields (usually private fields with public getters).
- Add either [CreateAssetMenuAttribute], [PreferencesAttribute], or [ProjectSettingsAttribute].
- Configure it in the editor.
- (Optional) If it is not an editor-only [ScriptableSettings] you can enable the `Preload` option in the inspector to add it to the `Preloaded Assets`.

[PreferencesAttribute]:<../Coimbra/PreferencesAttribute.cs>
[ProjectSettingsAttribute]:<../Coimbra/ProjectSettingsAttribute.cs>
[ScriptableSettings]:<../Coimbra/ScriptableSettings.cs>
[CreateAssetMenuAttribute]:<https://docs.unity3d.com/ScriptReference/CreateAssetMenuAttribute.html>
[ScriptableObject]:<https://docs.unity3d.com/ScriptReference/ScriptableObject.html>
