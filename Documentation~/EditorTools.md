# [Coimbra Framework](Index.md): Editor Tools

This package offers some tools available in `Tools/Coimbra Framework/` and an option to configure a startup scene while playing in the editor.

## Editor Startup Scene

Going to `Project Settings/Coimbra Framework` you will find [Editor Startup Scene Settings].

There you can configure a `Startup Scene` that will be used when entering play mode. If none is set, then the default editor behaviour takes place.

> The startup scene will only be used if attempting to enter play mode when a scene included in the build settings is currently open. This will allow you to still play from test scenes where you always can iterate faster.

## Menu Items:

The menu items available in `Tools/Coimbra Framework/` are:

- `Assert Serializable Types`: Asserts that all types that inherits from a serializable type also contains the [SerializableAttribute]. It will also log a message if everything is correct.
- `Clear Cache`: Deletes all [AssetBundle] content that has been cached.
- `Create Assets Assembly`: Generate assemblies for all scripts in the `Assets` folder while also taking into consideration `Editor` folders. Needs to be triggered everytime a new third-party is imported.
- `Force Reserialize Assets`: Forcibly load and re-serialize all assets, flushing any outstanding data changes to disk. Useful after upgrading engine versions to avoid unnecessary dirtying later on.
- `Request Script Reload`: The Unity Editor reloads script assemblies asynchronously on the next frame. This resets the state of all the scripts, but Unity does not compile any code that has changed since the previous compilation.
- `Reset Play Mode Start Scene`: Reset the [EditorSceneManager.playModeStartScene] back to null, if set.
- `Save Assets`: Saves all assets changes to disk. About the same as `File/Save Project` but also with a nice default shortcut at `Alt + Shift + S`.

[Editor Startup Scene Settings]:<../Coimbra.Editor/EditorStartupSceneSettings.cs>
[SerializableAttribute]:<https://docs.unity3d.com/ScriptReference/Serializable.html>
[AssetBundle]:<https://docs.unity3d.com/ScriptReference/AssetBundle.html>
[EditorSceneManager.playModeStartScene]:<https://docs.unity3d.com/ScriptReference/SceneManagement.EditorSceneManager-playModeStartScene.html>
