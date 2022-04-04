# CS Framework

Package of general utilities to be used with Unity development.

## List of Content

- Editor Startup Scene: Add easy way to always load a specific scene when going to play inside the editor.
- Editor Tools: Add some general utilities under `Tools/Coimbra Framework` menu.
- Field Attributes: `Disable`, `Indent`, or add a `MessageBox` to a serialized field.
- GameObject Behaviour: Create a behaviour to control a GameObject as if were extending the GameObject class itself.
- GameObject Extensions: Check if a GameObject `IsPersistent` and `GetOrCreateBehaviour` easily.
- GameObject Pools: Specialized pooling implementation for GameObjects.
- Int/Float Ranges: `IntRange` and `FloatRange` to be used instead of `Vector2` and `Vector2Int` when requiring a min and a max value.
- Interface Fields: With `ManagedField` you can expose a field to reference an interface, accepting anything that implements it.
- List Extensions: Add `Clear()` overload that accepts a new min capacity, useful when combined with `ManagedPool` API.
- Object Extensions: Add `GetValid()`, `IsValid()` and `TryGetValid()` methods to object class to have a safe way to work with abstractions in Unity Objects.
- Object Pooling: Use `ManagedPool` or `SharedManagedPool` for pooling any managed objects.
- References: Create a `Reference` for any value or even another reference.
- Service Locator: Implement the `ServiceLocator` pattern easily. And it comes with a few services already:
  - `IApplicationService`: Responsible for the built-in events and compatibility with Enter Play Mode Options.
  - `ICoroutineService`: Start or stop standard Unity coroutines from anywhere.
  - `IEventService`: Listen and invoke strongly-typed events. Some events are also provided by default:
    - `ApplicationFocusEvent`: Listen to Unity's ApplicationFocus callback;
    - `ApplicationPauseEvent`: Listen to Unity's ApplicationPause callback;
    - `ApplicationQuitEvent`: Listen to Unity's ApplicationQuit callback;
    - `FixedUpdateEvent`: Listen to Unity's FixedUpdate callback;
    - `LateUpdateEvent`: Listen to Unity's LateUpdate callback;
    - `UpdateEvent`: Listen to Unity's Update callback;
    - PlayerLoopTiming events: Listen to each PlayerLoopTiming available in the UniTask API.
  - `IPoolingService`: Leverages `GameObjectPool` by making those easily accessible from anywhere.
  - `ITimerService`: Start or stop timers from anywhere with the same precision as `Invoke` and `InvokeRepeating`.
- Scriptable Settings: Easily access ScriptableObjects from anywhere.

## Dependencies

- UniTask: https://github.com/Cysharp/UniTask
