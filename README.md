# Coimbra Framework

[![openupm](https://img.shields.io/npm/v/com.coimbrastudios.core?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.coimbrastudios.core/)

Package of general utilities to be used with Unity development.

## Known Issues

- Source generators on 2020.3 don't run on the first time you open the editor after deleting the `Library`. Restarting the editor fixes the issue.

## List of Content

- `Actor`: Meant to be used as the main script for any `GameObject`, as if we were extending the `GameObject` class itself.
- `AssetReferenceComponentRestriction`: Filter any `AssetReference` to only show `GameObject` with the specified components.
- Assets Asssembly Creator: - Use `Tools/Coimbra Framework/Create Assets Assembly` menu item to generate assemblies for all scripts in the `Assets` folder.
- `CopyBaseConstructorsAttribute`: quick create types with the same constructors as their parent.
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
- `MonoBehaviour` Listeners: components to be used to listen for `MonoBehaviour` callbacks:
    - `StartListener`
    - `FixedUpdateListener`
    - `LateUpdateListener`
    - `UpdateListener`
- Object Pooling: Use `ManagedPoolT<T>` for pooling any managed objects. It also comes with a few `SharedManagedPool` implementations:
    - `DictionaryPool`: clears the `Dictionary`.
    - `GUIContentPool`: clears the `GUIContet`.
    - `HashSetPool`: : clears the `HashSet`.
    - `ListPool`: clears the `List`.
    - `ManagedPool`: allows custom clearing through the `ISharedManagedPoolHandler` interface.
    - `QueuePool`: clears the `Queue`.
    - `StackPool`: clears the `Stack`.
    - `StringBuilderPool`: clears the `StringBuilder`.
- Property Attributes: Property attributes fully compatible with Unity's standard workflow:
    - `AnimatorParameter`: Draws a parameter selector for a given animator.
    - `AssetsOnly`: Prevents to assign a scene object to a `UnityEngine.Object` field.
    - `EnumFlags`: Turns an enum field into a enum mask popup field.
    - `IntRange`: Draws a property as if it was a `IntRange`.
    - `FloatRange`: Draws a property as if it was a `FloatRange`.
    - `LayerSelector`: Turns an int field into a layer popup field.
    - `NotGreaterThan`: Prevents an int field to have a value greater than the a given value.
    - `NotLessThan`: Prevents an int field to have a value smaller than the a given value.
    - `RangeSlider`: Draws a property using the Unity's `MinMaxSlider`.
    - `SortingLayerID`: Turns an int field into a sorting layer popup field.
    - `TagSelector`: Turns a string field into a tag popup field.
    - `Validate`: Calls a method `void()` or `void(T previous)` when the property is changed. It is also the base for all others attributes.
- `PropertyPathInfo`: Reflection helper class for any `SerializeField` based on its [propertyPath](https://docs.unity3d.com/ScriptReference/SerializedProperty-propertyPath.html).
- References: Create a `Reference` for any value or even another reference.
- Scriptable Settings: Easily access a `ScriptableObject` from anywhere with option to preload those on the application startup. You can also make them appear in the project settings with `ProjectSettingsAttribute` or in the preferences
  with `PreferencesAttribute`. You can see all the currently loaded `ScriptableSettings` in `Tools/Coimbra Framework/Windows/Scriptable Settings`;
- `SerializableDictionary`: supports modifying and saving through the inspector. Can have its size locked with `DisableResizeAttribute` or made read-only with `DisableAttribute`.
- Service Locator: Enable a service-based architecture easily. It also comes with a few built-in functionalities:
    - Attributes:
        - `DisableDefaultFactoryAttribute`: By default, a factory is set for each new compatible type during `SubsystemRegistration`. You can disable that per-implementation by using this attribute.
        - `PreloadServiceAttribute`: Add this in your `IService` implementation to call the `ServiceLocator.Shared.Get` during `BeforeSceneLoad`. This is just to reduce common boilerplate code.
    - Services:
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
| COIMBRA0002 | Concrete IEvent should not be a nested type.                                                          | Error    | Yes      |
| COIMBRA0003 | Class events should be either abstract or sealed.                                                     | Error    | No       |
| COIMBRA0004 | ServiceLocator APIs requires an interface type as generic parameter.                                  | Error    | No       |
| COIMBRA0005 | ServiceLocator APIs requires an interface type without AbstractServiceAttribute as generic parameter. | Error    | No       |
| COIMBRA0006 | Concrete IService should only implement one IService at a time.                                       | Error    | No       |
| COIMBRA0007 | Concrete IService should not implement any IService with AbstractServiceAttribute.                    | Warning  | No       |
| COIMBRA0008 | Type with SharedManagedPoolAttribute should not be generic.                                           | Error    | No       |
| COIMBRA0009 | IEventService generic APIs should not be used directly.                                               | Error    | No       |
| COIMBRA0010 | Type can't implement any IService because parent class already implements one.                        | Error    | No       |
| COIMBRA0011 | Concrete IService should not be a Component unless it inherit from ServiceActorBase.                  | Warning  | No       |
| COIMBRA0012 | A ScriptableSettings should not implement any IService.                                               | Error    | No       |
| COIMBRA0013 | ProjectSettingsAttribute and PreferencesAttribute should not be used together.                        | Error    | No       |
| COIMBRA0014 | ScriptableSettings has an invalided FileDirectory.                                                    | Error    | No       |
| COIMBRA0015 | ScriptableSettings attributes are not supported on abstract types.                                    | Error    | No       |
| COIMBRA0016 | ScriptableSettings attributes are not supported on generic types.                                     | Error    | No       |
| COIMBRA0017 | ServiceLocator.Shared should not be accessed inside IService.                                         | Error    | No       |
| COIMBRA0018 | OwningLocator.set is for internal use only.                                                           | Error    | No       |
| COIMBRA0019 | Object.Destroy should be avoided.                                                                     | Info     | No       |
| COIMBRA0020 | Type with CopyBaseConstructorsAttribute should be partial.                                            | Warning  | Yes      |
| COIMBRA0021 | Type with CopyBaseConstructorsAttribute should not be nested.                                         | Error    | Yes      |
| COIMBRA0022 | Type with SharedManagedPoolAttribute should be partial.                                               | Warning  | Yes      |
| COIMBRA0023 | Type with SharedManagedPoolAttribute should not be nested.                                            | Error    | Yes      |
