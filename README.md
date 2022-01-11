# CS Framework

Package of general utilities to be used with Unity development.

## List of Content

- Field Attributes: `Disable`, `Indent`, or add a `MessageBox` to a serialized field.
- GameObject Events: Listen to GameObject `SetActive` or `Destroy` events easily.
- Int/Float Ranges: `IntRange` and `FloatRange` to be used instead of `Vector2` and `Vector2Int` when requiring a min and a max value.
- Expose Interface Fields: With `ManagedField` you can expose a field to reference an inteface, accepting anything that implements it.
- Pooling: Use `ManagedPool` for pooling managed C# objects or create your own pool by inheriting from `ManagedPoolBase`.
- Reference: Create a `Reference` for any type.
- Service Locator: Implement the `ServiceLocator` pattern easily. And it comes with a few services already:

  - `ApplicationService`: Listen to `OnApplicationFocus`, `OnApplicationPause` and `OnApplicationQuit` easily from anywhere.
  - `CoroutineService`: Start or stop standard Unity coroutines from anywhere.
  - `EventService`: Register, listen and invoke strongly-typed events.
  - `TimerService`: Start or stop timers from anywhere with the same precision as `Invoke` and `InvokeRepeating`.