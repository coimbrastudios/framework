# CS Framework

Package of general utilities to be used with Unity development.

## List of Content

- Field Attributes: `Disable`, `Indent`, or add a `MessageBox` to a serialized field.
- GameObject Events: Listen to GameObject `SetActive` or `Destroy` events easily.
- Int/Float Ranges: `IntRange` and `FloatRange` to be used instead of `Vector2` and `Vector2Int` when requiring a min and a max value.
- Expose Interface Fields: With `ManagedField` you can expose a field to reference an interface, accepting anything that implements it.
- Pooling: Use `ManagedPool` for pooling managed C# objects or create your own pool by inheriting from `ManagedPoolBase`.
- Reference: Create a `Reference` for any type.
- Object Extensions: Add GetValid() and IsValid() methods to object class to have a safe way to work with abstractions in Unity Objects.
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
  - `ITimerService`: Start or stop timers from anywhere with the same precision as `Invoke` and `InvokeRepeating`.

## Dependencies

- UniTask: https://github.com/Cysharp/UniTask
