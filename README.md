# CS Framework

Package of general utilities to be used with Unity development.

## List of Content

- `Actor`: Meant to be used as the main script for any GameObject, as if we were extending the GameObject class itself.
- `AssetReferenceComponentRestriction`: Filter any `AssetReference` to only show GameObjects with the specified components.
- Editor Startup Scene: An easy way to always load a specific scene when going to play inside the editor This can be configured in `Project Settings/Coimbra Framework`.
- Editor Tools: Some general utilities under `Tools/Coimbra Framework` menu.
- Decorator Attributes: Decorator attributes fully compatible with Unity's standard workflow:
  - `Disable`: To disable a field so that it can't be edited. There is also `OnEditMode` and `OnPlayMode` versions.
  - `Indent`: To indent a field by the amount of steps specified.
  - `MessageBox`: To display a message above a field. There is also `OnEditMode` and `OnPlayMode` versions.
- GameObject Extensions: Easily check if a GameObject `IsPersistent` or fake-cast it to `Actor`.
- GameObject Pools: Specialized pooling implementation for GameObjects with auto resizing functionality.
- Int/Float Ranges: `IntRange` and `FloatRange` to be used instead of `Vector2` and `Vector2Int` when requiring a min and a max value.
- Interface Fields: With `ManagedField` you can expose a field to reference an interface, accepting anything that implements it.
- List Extensions: A few useful methods for common operations.
- Object Extensions: Methods to have a safe way to use `?.` and `??` operators in Unity Objects.
- Property Attributes: Property attributes fully compatible with Unity's standard workflow:
    - `IntRange`: Draws a property as if it was a `IntRange`.
    - `FloatRange`: Draws a property as if it was a `FloatRange`.
    - `RangeSlider`: Draws a property using the Unity's MinMaxSlider.
- `PropertyPathInfo`: Reflection helper class for any SerializeField based on its [propertyPath](https://docs.unity3d.com/ScriptReference/SerializedProperty-propertyPath.html).
- Object Pooling: Use `ManagedPool` or `SharedManagedPools` for pooling any managed objects.
- References: Create a `Reference` for any value or even another reference.
- Service Locator: Enable a service-based architecture easily. It comes with a few services already:
  - `IApplicationStateService`: Responsible for some built-in events:
    - `ApplicationFocusEvent`: Invoked on Unity's ApplicationFocus callback;
    - `ApplicationPauseEvent`: Invoked on Unity's ApplicationPause callback;
    - `ApplicationQuitEvent`: Invoked on Unity's ApplicationQuit callback;
  - `ICoroutineService`: Start or stop standard Unity coroutines from anywhere.
  - `IEventService`: Listen and invoke strongly-typed events.
  - `IPlayerLoopService`: Responsible for [PlayerLoop](https://docs.unity3d.com/ScriptReference/LowLevel.PlayerLoop.html)-related events:
    - `FixedUpdateEvent`: Invoked on Unity's FixedUpdate callback;
    - `LateUpdateEvent`: Invoked on Unity's LateUpdate callback;
    - `UpdateEvent`: Invoked on Unity's Update callback;
    - [PlayerLoopTiming](https://github.com/Cysharp/UniTask#playerloop) Events: Invoked in their respective timing.
  - `IPoolingService`: Leverages `GameObjectPool` by making those easily accessible from anywhere.
  - `ITimerService`: Start or stop timers from anywhere with the same precision as `Invoke` and `InvokeRepeating`.
- Scriptable Settings: Easily access ScriptableObjects from anywhere with option to preload those on the application startup. You can also make them appear in the project settings with `ProjectSettingsAttribute`.

## Dependencies

- UniTask: https://github.com/Cysharp/UniTask
