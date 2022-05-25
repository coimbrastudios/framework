# Coimbra Framework

[![openupm](https://img.shields.io/npm/v/com.coimbrastudios.core?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.coimbrastudios.core/)

Package of general utilities to be used with Unity development.

## Known Issues

- Source generators on 2020.3 don't run on the first time you open the editor after deleting the `Library`. Restarting the editor fixes the issue.

## List of Content

- `Actor`: Meant to be used as the main script for any `GameObject`, as if we were extending the `GameObject` class itself.
- `AssetReferenceComponentRestriction`: Filter any `AssetReference` to only show `GameObject` with the specified components.
- Decorator Attributes: Decorator attributes fully compatible with Unity's standard workflow:
    - `Disable`: To disable a field so that it can't be edited. There is also `OnEditMode` and `OnPlayMode` versions.
    - `Indent`: To indent a field by the amount of steps specified.
    - `MessageBox`: To display a message above a field. There is also `OnEditMode` and `OnPlayMode` versions.
- Editor Startup Scene: An easy way to always load a specific scene when going to play inside the editor This can be configured in `Project Settings/Coimbra Framework`.
- Editor Tools: Some general utilities under `Tools/Coimbra Framework` menu.
- `FormerlySerializedAsBackingFieldOfAttribute`: `FormerlySerializedAs` subclass that correctly formats the input to `<{propertyName}>k__BackingField`.
- GameObject Pools: Specialized pooling implementation for `GameObject` with auto resizing functionality.
- Int/Float Ranges: `IntRange` and `FloatRange` to be used instead of `Vector2` and `Vector2Int` when requiring a min and a max value.
- Interface Fields: With `ManagedField` you can expose a field to reference an interface, accepting anything that implements it. Combine it with `TypeFilterAttribute` to better control which objects can be referenced.
- Object Pooling: Use `ManagedPoolT<T>` for pooling any managed objects. It also comes with a few `SharedManagedPool` implementations:
    - `DictionaryPool`: clears the `Dictionary`.
    - `GUIContentPool`: clears the `GUIContet`.
    - `HashSetPool`: : clears the `HashSet`.
    - `ListPool`: clears the `List`.
    - `ManagedPool`: allows custom clearing through the `ISharedManagedPoolHandler` interface, while also deleting `IDisposable` and `UnityEngine.Object` instances correctly.
    - `QueuePool`: clears the `Queue`.
    - `StackPool`: clears the `Stack`.
    - `StringBuilderPool`: clears the `StringBuilder`.
- Property Attributes: Property attributes fully compatible with Unity's standard workflow:
    - `IntRange`: Draws a property as if it was a `IntRange`.
    - `FloatRange`: Draws a property as if it was a `FloatRange`.
    - `RangeSlider`: Draws a property using the Unity's `MinMaxSlider`.
- `PropertyPathInfo`: Reflection helper class for any `SerializeField` based on its [propertyPath](https://docs.unity3d.com/ScriptReference/SerializedProperty-propertyPath.html).
- References: Create a `Reference` for any value or even another reference.
- Scriptable Settings: Easily access a `ScriptableObject` from anywhere with option to preload those on the application startup. You can also make them appear in the project settings with `ProjectSettingsAttribute` or in the preferences with `PreferencesAttribute`.
- `SerializableDictionary`: view, edit and save dictionaries from the inspector. Also supports nesting and lists.
- Service Locator: Enable a service-based architecture easily. It comes with a few services already:
    - `IApplicationStateService`: Responsible for some built-in events:
        - `ApplicationFocusEvent`
        - `ApplicationPauseEvent`
        - `ApplicationQuitEvent`
    - `ICoroutineService`: Start or stop a Unity `Coroutine` from anywhere.
    - `IEventService`: Listen and invoke strongly-typed events.
    - `IPlayerLoopService`: Responsible for [PlayerLoop](https://docs.unity3d.com/ScriptReference/LowLevel.PlayerLoop.html)-related events:
        - `FixedUpdateEvent`
        - `LateUpdateEvent`
        - `UpdateEvent`
        - [PlayerLoopTiming](https://github.com/Cysharp/UniTask#playerloop) Events
    - `IPoolingService`: Leverages `GameObjectPool` by making those easily accessible from anywhere.
    - `ITimerService`: Start or stop timers from anywhere with the same precision as `Invoke` and `InvokeRepeating`.
- Type Dropdown: Use `TypeDropdownAttribute` in combination with `SerializeReferenceAttribute` to expose a type selector. Can also be combined with `TypeFilterAttribute`.
- Utilities & Extensions: Check if a `GameObject` `IsPersistent`, `Destroy` any Unity `Object` safely, fake-cast a `GameObject` to `Actor`, use `?.` and `??` safely wth any Unity `Object`, and much more.

## Dependencies

- UniTask: https://github.com/Cysharp/UniTask

## Analyzers

| ID          | Title                                                                                                 | Severity | Code Fix |
|-------------|:------------------------------------------------------------------------------------------------------|----------|----------|
| COIMBRA0001 | Concrete IEvent should be partial.                                                                    | Warning  | Yes      |
| COIMBRA0002 | Concrete IEvent should not be a nested type.                                                          | Warning  | Yes      |
| COIMBRA0003 | Class events should be either abstract or sealed.                                                     | Warning  | No       |
| COIMBRA0004 | ServiceLocator APIs requires an interface type as generic parameter.                                  | Error    | No       |
| COIMBRA0005 | ServiceLocator APIs requires an interface type without AbstractServiceAttribute as generic parameter. | Error    | No       |
| COIMBRA0006 | Concrete IService should only implement one IService at a time.                                       | Error    | No       |
| COIMBRA0007 | Concrete IService should not implement any IService with AbstractServiceAttribute.                    | Warning  | No       |
| COIMBRA0008 | AbstractServiceAttribute should only be used with an interface that extends IService.                 | Warning  | No       |
| COIMBRA0009 | IEventService generic APIs should not be used directly.                                               | Error    | No       |
