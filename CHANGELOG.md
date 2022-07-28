# Changelog

## [10.9.9] - 2022-07-28

- Fixed default assembly definition rule assets causing unnecessary warning.

## [10.9.8] - 2022-07-27

- Added analyzer `COIMBRA0108`.
- Added analyzer `COIMBRA0109`.
- Added analyzer `COIMBRA0110`.
- Added `FixDuplicateReferencesAssemblyDefinitionRule`.

## [10.8.8] - 2022-07-27

- Added analyzer `COIMBRA0108`.
- Added analyzer `COIMBRA0109`.
- Added analyzer `COIMBRA0110`.
- Added `FixDuplicateReferencesAssemblyDefinitionRule`.

## [10.9.7] - 2022-07-27

- Added `NotNullWhen` on some `ServiceLocator`, `ScriptableSettings` and `ObjectUtility` APIs.

## [10.9.6] - 2022-07-22

- Added `ForceRootNamespaceMatchNameAssemblyDefinitionRule`.
- Added missing info in [Index](Documentation~/Index.md) for `RequiredServiceAttribute`.
- Fixed `ServiceLocator.GetChecked` failing for destroyed actors instead of falling back to the factory.
- Removed dependency on `com.unity.ide.vscode` as it is now [officially unsupported](https://forum.unity.com/threads/update-on-the-visual-studio-code-package.1302621/).

## [10.8.7] - 2022-07-22

- Added `ForceRootNamespaceMatchNameAssemblyDefinitionRule`.
- Added missing info in [Index](Documentation~/Index.md) for `RequiredServiceAttribute`.
- Fixed `ServiceLocator.GetChecked` failing for destroyed actors instead of falling back to the factory.
- Removed dependency on `com.unity.ide.vscode` as it is now [officially unsupported](https://forum.unity.com/threads/update-on-the-visual-studio-code-package.1302621/).

## [10.9.5] - 2022-07-21

- Added missing info in [Index](Documentation~/Index.md) for `Assembly Definition Rules` added in the last version.
- Added `RequiredServiceAttribute` to enable code to assume tha that a service is never null without requiring to be dynamic.
- Added assertion for required services inside ServiceLocator.GetChecked.
- Added `[RequiredService]` for all default services.
- Added `EventHandleListUtility.RemoveListenersAndClear` to make easier to clear a list of `EventHandle`.
- Changed `IServiceFactory.Create` to `IServiceFactory.GetService` as not always it will be actually creating a new service.
- Changed `DefaultServiceActorFactory` to try to use `Object.FindObjectOfType` before creating a new instance.
- Changed [RoslynAnalyzers](Documentation~/RoslynAnalyzers.md) documentation to be more readable and include planned analyzers.
- Changed diagnostics to be split into different classes to match their new ID assignment.

## [10.8.6] - 2022-07-21

- Added `RequiredServiceAttribute` to enable code to assume tha that a service is never null without requiring to be dynamic.
- Added assertion for required services inside ServiceLocator.GetChecked.
- Added `[RequiredService]` for all default services.
- Added `EventHandleListUtility.RemoveListenersAndClear` to make easier to clear a list of `EventHandle`.
- Added `FixEditorOnlyAssemblyDefinitionRule`.
- Added `FixTestsOnlyAssemblyDefinitionRule`.
- Added `ForceReferencyByNameAssemblyDefinitionRule`.
- Added `Delayed<T>` wrapper struct.
- Added `ISceneProcessorComponent` and `IScenePostProcessorComponent` interfaces to enable scene-level processing logic in runtime scripts.
- Added `DebugOnly` component to allow creation of actors that should only exist in the editor or in development builds.
- Added `HierarchyFolder`, a special kind of actor that re-parent all its children and destroys itself at runtime.
- Changed `IServiceFactory.Create` to `IServiceFactory.GetService` as not always it will be actually creating a new service.
- Changed `DefaultServiceActorFactory` to try to use `Object.FindObjectOfType` before creating a new instance.
- Changed [RoslynAnalyzers](Documentation~/RoslynAnalyzers.md) documentation to be more readable and include planned analyzers.
- Changed diagnostics to be split into different classes to match their new ID assignment.
- Changed `Coimbra.Linting.Editor` to `Coimbra.Editor.Linting` as it is an editor-only feature.
- Changed `BannedReferencesAssemblyDefinitionRule` to use paths instead of names.
- Changed `LintingSettings` and `UPMAuthenticator` to use the `Assets` instead.
- Changed `Actor.Initialize` to cancel initialization as soon that its `Destroy` method is called (i.e. inside `ActorComponent.PreInitialize` or `ActorComponent.PostInitialize`).
- Fixed `RequiredReferencesAssemblyDefinitionRule` not checking the assembly name.
- Fixed `SortReferencesByNameAssemblyDefinitionrule` not sorting correctly when not using GUIDs.
- Removed `Actor.DeactivateOnInitializePrefab` as there was no support for undoing that operation when exiting play mode.

## [10.9.4] - 2022-07-14

- Added `FixEditorOnlyAssemblyDefinitionRule`.
- Added `FixTestsOnlyAssemblyDefinitionRule`.
- Added missing info in [Index](Documentation~/Index.md) for `Assembly Definition Rules`.

## [10.9.3] - 2022-07-11

- Added `ForceReferencyByNameAssemblyDefinitionRule`.
- Changed `Coimbra.Linting.Editor` to `Coimbra.Editor.Linting` as it is an editor-only feature.
- Fixed `RequiredReferencesAssemblyDefinitionRule` not checking the assembly name.
- Fixed `SortReferencesByNameAssemblyDefinitionrule` not sorting correctly when not using GUIDs.

## [10.9.2] - 2022-07-11

- Fixed analyzers running on unwanted assemblies.

## [10.9.1] - 2022-07-04

- Added missing info in [Index](Documentation~/Index.md) for `HierarchyFolder` and `DebugOnly`.
- Changed `BannedReferencesAssemblyDefinitionRule` to use paths instead of names.

## [10.9.0] - 2022-07-04

- Added `Delayed<T>` wrapper struct.
- Added `ISceneProcessorComponent` and `IScenePostProcessorComponent` interfaces to enable scene-level processing logic in runtime scripts.
- Added `DebugOnly` component to allow creation of actors that should only exist in the editor or in development builds.
- Added `HierarchyFolder`, a special kind of actor that re-parent all its children and destroys itself at runtime.
- Changed `COIMBRA0011` to be an error.
- Changed `COIMBRA0019` to be an error.
- Changed minimum Unity version to 2021.3.5f1.
- Changed scripts to use the new C# language features.
- Changed Roslyn DLLs version to `3.9.0` to match Unity upgrade.
- Changed `LintingSettings` and `UPMAuthenticator` to use the `Assets` instead.
- Changed `Actor.Initialize` to cancel initialization as soon that its `Destroy` method is called (i.e. inside `ActorComponent.PreInitialize` or `ActorComponent.PostInitialize`).
- Removed dependency on `com.unity.roslyn`.
- Removed `Actor.DeactivateOnInitializePrefab` as there was no support for undoing that operation when exiting play mode.

## [10.8.5] - 2022-06-30

- Added `HideKeyLabelAttribute` and `HideValueLabelAttribute` to use with `SerializableDictionary`.
- Fixed some incorrect indentation for some fields.
- Fixed nested `SerializeReference` not working with `PropertyPathInfo`.

## [10.8.4] - 2022-06-28

- Added context parameter to `AssemblyDefinitionRuleBase.Apply` to make troubleshooting easier.
- Changed [LICENSE](LICENSE.md) to MIT.
- Fixed some `ScriptableSettings` not writing to disk correctly when being saved.

## [10.8.3] - 2022-06-27

- Added assembly definition rules to reorder the references.
- Fixed `AssemblyDefinition` serialization.

## [10.8.2] - 2022-06-27

- Added `Coimbra.Linting` and `Coimbra.Linting.Editor` to prepare for `StyleCop.Analyzers` support.
- Added basic linting support for assembly definition files.
- Changed `AssemblyDefinition` and `AssemblyDefinitinoReference` types to be public.
- Changed listener classes to a new assembly `Coimbra.Listeners`.
- Changed ui classes to new assembly `Coimbra.UI`.

## [10.8.1] - 2022-06-23

- Fixed events outside the `Coimbra` namespace being generated with errors.

## [10.8.0] - 2022-06-22

- Added online documentation link.
- Added online license link.
- Added online changelog link.
- Added support for dynamic `PropertyPathInfo`. Those aren't cached but fully supports inheritance trees.
- Added debug information for `EventSystem`.
- Added debug information for `PoolSystem`.
- Added Managed Jobs APIs.
- Changed [README](README.md) to only contains some basic information, details were moved to the new [documentation](Documentation~/Index.md).
- Removed support for int fields for `AnimatorParameterAttribute`.

## [10.7.11] - 2022-06-21

- Added missing [LICENSE](LICENSE.md).

## [10.7.10] - 2022-06-21

- Fixed missing `System` directive at `CoimbraEditorUserSettings` in Unity 2021.

## [10.7.9] - 2022-06-20

- Added `SelectableLabelAttribute` and its drawer.
- Added information about the listeners at the `EventHandle` and `EventSystemDrawer`.
- Added information about the callback listeners at the `TimerHandleDrawer` and `TimerComponentEditor`.
- Added `IEventService` APIs to the listener list for any given event or `EventHandle`.
- Changed new `IsTimerActive` overload to `GetTimerData` to make its behaviour more obvious.
- Fixed assembly creator not making the generated editor assembly reference the generated runtime assembly.
- Fixed `SerializableDictionary` not working properly when used as an array or list.

## [10.7.8] - 2022-06-19

- Added `Active Timers` profiler counter.
- Added some debug information to `TimerComponent` inspector.
- Added drawer for `TimerHandle`.
- Added `ITimerService.IsTimerActive` overload that also returns the timer data.

## [10.7.7] - 2022-06-18

- Added limitations description to `TypeDropdownAttribute` and `ManagedField` summaries.
- Added `Window/Coimbra Framework/Shared Managed Pools`.
- Added `ManagedPool<T>.AvailableCount` API.
- Added `COIMBRA0017`.
- Added `COIMBRA0018`.
- Changed `ISharedManagedPoolHandler` to `IManagedPoolHandler`.
- Changed `TypeFilterAttributeBase` to `FilterTypeAttributeBase` and added support for multiple attributes in the same field.
- Changed `TypeFilterAttribute` to `FilterTypeAByAssignableFromttribute` and made it sealed.

## [10.7.6] - 2022-06-17

- Fixed warning due unnecessary reference to `Unity.Settings.Editor`.

## [10.7.5] - 2022-06-17

- Added helper methods at `ScriptableSettingsEditor`.

## [10.7.4] - 2022-06-17

- Added basic search support for `ScriptableSettings`.

## [10.7.3] - 2022-06-17

- Added `Project Settings/Coimbra Framework/Editor` with option to disable local analyzers.

## [10.7.2] - 2022-06-16

- Added overlap 2D listeners.
- Changed `FixedUpdateListener` implementation to internal.
- Changed `LateUpdateListener` implementation to internal.
- Changed `UpdateListener` implementation to internal.

## [10.7.1] - 2022-06-16

- Added `ActorComponentBase` to easily add initialization logic on components that depends on `Actor`.

## [10.7.0] - 2022-06-16

- Added listener components for all remaining callbacks.
- Added `HideInServiceLocatorWindowAttribute` due test services appearing in the window.
- Added `PropertyType` to `PropertyPathInfo` and better document what exactly `FieldInfo` refers to.
- Added `SerializableType` which is just a serialization wrapper for `System.Type`.
- Added `CoimbraEditorGUIUtility.DrawPropertField` and `CoimbraEditorGUIUtility.GetPropertyHeight` APIs that always use the field's type drawer.
- Added `EventContext` and `EventContextHandler` types, they replace the old `Event<T>` and `Event<T>.Handler`.
- Added extra debug information for when an exception is thrown when invoking an event.
- Added `IPlayerLoopService.AddListener` to allow registering to any `IPlayerLoopEvent` by its type.
- Added `IDisposable` interface to `Actor` and make `Dispose` call `Destroy`.
- Added `ServiceLocator.GetChecked<T>()` that both gets and asserts that the value is valid.
- Changed `ServiceLocator` to not allow overriding a service value once set. This can be disabled per-service with the new `DynamicServiceAttribute`.
- Changed `COIMBRA0011` to refer to `Actor` instead of the now removed `ServiceActorBase`.
- Changed `EventRelevancyChangedHandle` to not be a nested type of `IEventService`.
- Fixed `ValidateDrawer` not working with types that had a custom `PropertyDrawer`.
- Removed `ServiceActorBase` as `Actor` already implements `IDisposable` properly now.
- Removed `Event<T>` type as it got replaced by new APIs.

## [10.6.0] - 2022-06-14

- Added `AssetReferenceScene` for referencing `SceneAsset` in runtime code.
- Added `AnimatorIKListener` and `AnimatorMoveListener`. 
- Added `BecameVisibleListener` and `BecameInvisibleListener`.
- Added Collision/Trigger Enter/Exit/Stay Listeners for 2D and 3D colliders.
- Added `ControllerColliderHitListener`.
- Added `JointBreakListener` and `JointBreak2DListener`.
- Added `WillRenderObjectListener`.
- Added `TransformChangedListener`.
- Changed `ServiceLocator` to be a static class.
- Changed codegen for `IEvent` to match the new `ServiceLocator` implementation.
- Removed support for multiple `ServiceLocator` instances.

## [10.5.0] - 2022-06-13

- Added `Window/Coimbra Framework/Scriptable Settings` to easily view all loaded `ScriptableSettings`.
- Added `Window/Coimbra Framework/Service Locators` to easily view all created `ServiceLocator`.
- Added support for `PreferencesAttribute` with a `null` window path. This will make the `ScriptableSettings` hidden in the editor (useful for temporary or internal configurations). 
- Added components to be used to listen for `MonoBehaviour` callbacks: `StartListener`, `FixedUpdateListener`, `LateUpdateListener`, `UpdateListener`.
- Added `Add/RemoveSetListener` APIs to `ServiceLocator`.
- Added `PlayerLoopSettings` to allow some project-specific optimizations for the `PlayerLoopSystem`.
- Changed COIMBRA0019 to be a warning and improve its analysis to only report if the type is `GameObject`, any `Actor`, or specifically `UnityEngine.Object`.
- Changed internal implementation of `Clear Console On Reload` and `Startup Scene Manager` to use the `ScriptableSettings` API to make the code more maintainable.
- Changed `ManagedFieldDrawer` to hide the `Clear` button when being draw inside a disabled scope.
- Changed `ServiceActorBase` to not be re-usable across different `ServiceLocator`.
- Changed `ServiceLocator.Set` API to not allow overriding an existing value with another value (can only set to `null` is any other value is set).
- Fixed bug with some `ScriptableSettings` not rendering properly after entering or existing play mode.
- Removed `Add/RemoveValueChangedListener` APIs from `ServiceLocator`. Those got replaced be the new `Add/RemoveSetListener`.

## [10.4.2] - 2022-06-02

- Changed documentation for `ServieActorBase.OnOwningLocatorChanged` to include when it is called and how should be used.
- Changed `ServieActorBase.OnDestroyed` to be sealed, a new virtual `OnDispose` method was included and should be used instead.

## [10.4.1] - 2022-06-02

- Added dependency on `com.unity.profiling.core`.
- Added profile counters for `Actor` usage.
- Changed generated static `Invoke` method to `InvokeDefault` to make its usage more clear.

## [10.4.0] - 2022-06-01

- Changed `COIMBRA0003` to be error instead of warning.
- Changed event methods generation to be more intuitive to use. It is a breaking change and requires modifying any code using the previous code generation. I shouldn't touch that logic anymore as this is becoming a never ending stuff.

## [10.3.7] - 2022-05-31

- Added `AnimatorParameterAttribute`.
- Added `AssetsOnlyAttribute`.
- Added `EnumFlagsAttribute`.
- Added `LayerSelectorAttribute`.
- Added `NotGreaterThanAttribute`.
- Added `NotLessThanAttribute`.
- Added `SortingLayerIDAttribute`.
- Added `TagSelectorAttribute`.
- Added `ValidateAttribute`.

## [10.3.6] - 2022-05-30

- Added `AssertAwake` and `IsAwaken` properties to `Actor`.
- Added analyzer `COIMBRA0008`.
- Added analyzer `COIMBRA0020` and code fix.
- Added analyzer `COIMBRA0021` and code fix.
- Added analyzer `COIMBRA0022` and code fix.
- Added analyzer `COIMBRA0023` and code fix.
- Changed `LockDictionarySizeAttribute` to `DisableResizeAttribute`.
- Changed `SerializableDictionaryDrawer` implementation to improve its usability and readability.
- Fixed assembly creator causing compiler errors when re-opening the project.

## [10.3.5] - 2022-05-28

- Added `Create Assets Assembly` menu item to generate assemblies for all scripts in the `Assets` folder.

## [10.3.4] - 2022-05-27

- Added `CopyBaseConstructorsAttribute` to allow easy copying of parent constructors into child class.

## [10.3.3] - 2022-05-27

- Added `AllowEventServiceUsageForAttribute` to allow using `IEventService` methods directly when inside specific types.
- Removed analyzer `COIMBRA0008` due it being redundant. Using `BaseTypeRequiredAttribute` instead.

## [10.3.2] - 2022-05-26

- Added `LockDictionarySizeAttribute` to be used with `SerializableDictionary`.
- Added analyzer `COIMBRA0010`.
- Added analyzer `COIMBRA0011`.
- Added analyzer `COIMBRA0012`.
- Added analyzer `COIMBRA0013`.
- Added analyzer `COIMBRA0014`.
- Added analyzer `COIMBRA0015`.
- Added analyzer `COIMBRA0016`.
- Added analyzer `COIMBRA0017`.
- Added analyzer `COIMBRA0018`.
- Added analyzer `COIMBRA0019`.

## [10.3.1] - 2022-05-25

- Added `Force Reserialize Assets` menu item.
- Changed `CachedGameObject` to just `GameObject` and executes additional logic inside to return the correct value according to the context.
- Changed `CachedTransform` to just `Transform` and executes additional logic inside to return the correct value according to the context.
- Fixed missing `Initialize` call on `GameObjectPool.LoadAsync`.
- Fixed missing dependency on newer IDE packages.

## [10.3.0] - 2022-05-25

- Added `FormerlySerializedAsBackingFieldOfAttribute` that correctly formats the name to match the format `<{propertyName}>k__BackingField`.
- Added `DebuggerStepThroughAttribute` where applicable.
- Added `EditorBrowsableAttribute(EditorBrowsableState.Never)` where applicable.
- Added `DisableDefaultFactoryAttribute` to be used with `IService` implementations.
- Added `PreloadServiceAttribute` to be used with `IService` implementations.
- Changed `CreateCallback` APIs to a `Factory` pattern on `ServiceLocator`.
- Changed static `ManagedPool` to require the `IDisposable` interface indirectly through the `ISharedManagedPoolHandler` interface.
- Fixed `Disposable<T>` allowing multiple `Dispose` calls.

## [10.2.10] - 2022-05-24

- Added `ReferenceDrawer` to correctly expose a `Reference<T>` in the inspector.
- Added `TypeDropdownAttribute` to be used with `SerializeReference` fields.
- Added `TypeFilterAttribute` to filter available types for `ManagedField<T>` dropdown.
- Added check for editor-only `ScriptableSettings` in the `Assets` folder.
- Added many test cases for `PropertyPathInfo` to ensure its stability.
- Changed `PropertyPathInfo` internal implementation, improving its performance, memory usage, and exposing some new APIs.
- Changed `ManagedFieldDrawer` implementation to use a better dropdown menu that supports searching.

## [10.2.9] - 2022-05.20

- Changed `Startup Scene Index` to `Startup Scene` instead, becoming a hard-reference and allowing disabled scenes too.
- Fixed unnecessary warnings on `UPMAuthenticator`.
- Removed generated `static Invoke` overloads that took the data by ref as all use cases could use the instance methods instead.

## [10.2.8] - 2022-05-19

- Added `ISharedManagedPoolHandler` and make it required for the default shared `ManagedPool` implementation.
- Changed `try/catch` block on `EventSystem` to first log the the event type that emitted the error then the exception itself.
- Fixed `Startup Scene Index` picking disabled scenes.

## [10.2.7] - 2022-05-18

- Fixed missing dependency on `com.unity.ugui` package.

## [10.2.6] - 2022-05-18

- Fixed missing reference to `UnityEngine.UI` in `Coimbra` assembly.

## [10.2.5] - 2022-05-18

- Added `UPMAuthenticator` to automatically update the `.upmconfig.toml` for all users of a team.
- Fixed one of the `ProjectSettingsAttribute` and `PreferencesAttribute` constructors overload not working as expected.

## [10.2.4] - 2022-05-18

- Added `PreferencesAttribute` that can be applied to any `ScriptableSettings` to make it editor-only and show up in the `Preferences` window.
- Fixed `SerializableDictionary` not showing custom structs without a `CustomPropertyDrawer` correctly.

## [10.2.3] - 2022-05-18

- Added option to make a `ScriptableSettings` editor-only through the `ProjecSettingsAttribute`.

## [10.2.2] - 2022-05-16

- Added back the menu item `Tools/Coimbra Framework/Preferences/Clear Console On Reload` for Unity 2020.3.
- Added `SerializableDictionary` to allow viewing and modifying dictionaries in the inspector (with nesting support).
- Added `GeneratedExtensions` class with `InvokeAt`, `TryInvokeAt`, and `TryInvokeShared` methods.
- Added `Try` prefix for all `Shared` method versions to better indicate the implicit null-check.
- Added `Try` variant for all `At` method versions to better indicate the implicit null-check.
- Removed `TryInvokeAt` and `TryInvokeShared` instance methods as `GeneratedExtensions` made those obsolete.

## [10.2.1] - 2022-05-14

- Fixed compatibility with 2020.3 by removing duplicated Roslyn assemblies.

## [10.2.0] - 2022-05-14

- Added easy way to add new shared `ManagedPool` instances through `SharedManagedPoolAttribute`.
- Added some specific pool collections: `DictionaryPool`, `HashSetPool`, `ListPool`, `QueuePool`, `StackPool`, `StringBuilderPool`.
- Removed support for `IList` and `IDictionary` in `ManagedPool` class, use the respective specific pool instead (see `COIMBRA0010`).

## [10.1.2] - 2022-05-13

- Added `Object.Destroy` extension method for safely destroy object that could be a `GameObject` or an `Actor`.

## [10.1.1] - 2022-05-12

- Added `GameObject.Destroy` extension method for safely destroy objects that can be either already destroyed, already an actor or outside play mode.
- Added null check inside `IEvent` generated `Shared` methods.
- Fixed `IEvent` generated `At` methods not requiring non-null service.
- Fixed `PropertyPathInfo` cache not being shared between different instances of same type.
- Fixed conditional expressions being ignored in the `IEventService` usage analyzer.

## [10.1.0] - 2022-05-12

- Added `IApplicationStateService.RemoveAllListeners` method.
- Added `IPlayerLoopService.RemoveAllListeners` method.
- Added `IEventService.AddRelevancyListener` and `IEventService.RemoveRelevancyListener` methods.
- Changed to not remove all listeners by default during `ApplicationStateSystem.OnDestroyed`.
- Changed to not remove all listeners by default during `PlayerLoopSystem.OnDestroyed`.
- Fixed generic events not being correctly generated.
- Removed `IEventService.OnFirstListenerAdded` and `IEventService.OnLastListenerRemoved`.

## [10.0.0] - 2022-05-11

- Added new diagnostics to identify wrong usage of `IEventService`.
- Added missing documentation for `AbstractServiceAttribute`.
- Added `DespawnCancellationToken` and `DestroyCancellationToken` properties to `Actor`.
- Added `GetListenerCount` to event-related APIs.
- Changed back the minimum Unity version to 2020.3.
- Changed for each system to be in its own assembly.
- Removed `HasAnyListeners` from event-related APIs.

## [9.0.0] - 2022-04-26

- Added source generator and roslyn analyzers for `IEvent` to improve API usage and debugging experience.
- Added roslyn analyzers for `IService` related APIs.
- Added `IEventService.IsInvoking` API to check if an event is currently invoking.
- Added throw/catch during `EventSystem.Invoke` to make easier to debug issues during invocation.
- Added `ServiceLocator.IsCreated` overloads with `out` parameters to receive the service value.
- Added globals `Actor.OnSceneInitialized` and `Actor.OnSceneInitializedOnce` event to use for new loaded scenes instead of Start.
- Added `IsFocused` and `IsPaused` properties in `IApplicationService`.
- Added `IEventService.OnFirstListenerAdded` and `IEventService.OnLastListenerRemoved` events.
- Added `IPlayerLoopService` to reduce the responsibilities from `IApplicationService`.
- Added `EventServiceActorBase` class.
- Changed minimum Unity version to 2021.3.
- Changed `EventData` to `Event` and added a `Service` field on it.
- Changed `Actor` destruction event to happen before virtual method call, switching their names to match the new behaviour.
- Changed `IApplicationService` to `IApplicationStateService` and reduced its responsibilities.
- Changed `ServiceLocator.Get` to not set the service value when using the fallback option as it was misleading.
- Changed `ServiceLocator.Set` to early-out on same service value and to use the fallback option for the value changed event.
- Changed `OnDespawn` and `OnSpawn` to call `SetActive` by default.
- Changed `As` and `Is` GameObject extensions to `AsActor` and `IsActor`.
- Changed `ServiceLocator.Dispose` to be called in builds too inside during Application.quitting.
- Changed all service-related stuff to a new assemblies and namespaces under `Coimbra.Services`.
- Changed `EventSystem.Invoke` to early out if there is no listeners.
- Changed `RemoveAllListenersWithoutRestriction` to `RemoveAllListenersAllowed`.
- Changed `SharedManagedPool` to `SharedManagedPools` to make the name more explicit.
- Fixed `ProjectSettings` generated windows not updating correctly right after creating or destroying the target instance.
- Fixed destroyed actors due scene unload being flagged as explicit call.
- Removed `Actor.InitializeAllActors`, now it gets called before the newly added events.
- Removed `SceneLoadedEvent`, use the newly added global `Actor` events instead.

## [8.0.0] - 2022-04-11

- Added `IPoolingService.LoadingPoolCount` property to efficiently check the amount of pool currently in the `Loading` state.
- Added `IPoolingService.ContainsPool(AssetReferenceT<GameObject>)` to efficiently check if any currently added pool is referencing the specified prefab.
- Added `Actor.IsPrefab` property that is initialized property during `Actor.Initialize()`.
- Added automatic resizing options for `GameObjectPool` through the new `GameObjectPool.ExpandStep` and `GameObjectPool.ShrinkStep` properties.
- Added `AssetReferenceComponentRestriction` attribute to filter any AssetReferenceT<GameObject> by its components.
- Added many new methods in `ListUtility`.
- Added `GameObjectPoolSceneManagerAPI` to leverage the `GameObjectPool` functionalities.
- Added `disposeCallback` parameter on `ManagedPool` to add logic after `OnDelete` is fired (i.e. native object disposing).
- Added proper support for IDisposable and UnityEngine.Object (i.e. ScriptableObject) in `SharedManagedPool`.
- Added option to modify the find behaviour of `ScriptableSettings` in `GetOrFind` and `TryGetOrFind`, but preserving the old behaviour if none is specified.
- Added `ProjectSettingsAttribute` and `ScriptableSettingsProvider` for easily displaying any `ScriptableSettings` in the project settings.
- Changed name of `FrameworkXXX` classes to `CoimbraXXX`.
- Changed `DestroyReason` to be a nested enum of `Actor`.
- Changed `GameObjectPool` and `Actor` implementation quite a bit:
    - Added public `Actor.Despawn()` to be used instead of `GameObjectPool.Despawn(Actor)`.
    - Added new `GameObjectPool.DesiredAvailableInstancesRange` property.
    - Changed `GameObjectPool.AutoLoad` property to `GameObjectPool.LoadOnInitialize` to make its behaviour more explicit.
    - Changed `Actor.OnObjectDespawn()` to `Actor.OnDespawn()`.
    - Changed `Actor.OnObjectDestroy()` to `Actor.OnDestroying()`.
    - Changed `Actor.OnObjectInitialize()` to `Actor.OnInitialize()`.
    - Changed `Actor.OnObjectSpawn()` to `Actor.OnSpawn()`.
    - Changed `Actor.OnPrefabInitialize()` to `Actor.OnInitializePrefab()`.
    - Changed `GameObjectPool.OnObjectInstantiated` event to `GameObjectPool.OnInstanceCreated`, renaming also its matching delegate.
    - Changed `GameObjectPool.OnStateChanged` event to `GameObjectPool.OnPoolStateChanged`, renaming also its matching delegate.
    - Changed `GameObjectPool.Spawn` to assume that all available instances are valid.
    - Removed protected `Actor.Spawn()` and `Actor.Despawn()` methods.
    - Removed `GameObjectPool.Despawn` methods in favor of `Actor.Despawn()`.
    - Removed `GameObjectPool.DespawnResult` enum completely as now a despawn will always either return the object to its pool or destroy it.
    - Removed `GameObjectPool.MaxCapacity` property in favor of `GameObjectPool.DesiredAvailableInstancesRange.Max` property.
    - Removed `GameObjectPool.PreloadCount` property, with `GameObjectPool.DesiredAvailableInstancesRange.Max` property being used for the same purpose now.
- Fixed completely broken `PoolingSettings.DefaultPersistentPools` feature.
- Removed `IPoolingService.Despawn` methods in favor of new `Actor.Despawn()`.
- Removed `IPoolingService.PoolsLoading` property, if this information is needed it is still possible to use `IPoolingService.GetAllPools` and iterate it to check the `GameObjectPool.CurrentState`.

## [7.0.0] - 2022-04-05

- Added `SceneChangedEvent` that is also fired by `IApplicationService`.
- Added `IsQuitting` on `IApplicationService`.
- Added support for in-scene deactivated `Actor` initialization.
- Changed `ServiceBase<>` to `ServiceActorBase<>`.
- Changed `Spawn` APIs to return an `Actor` instead of `GameObject`.
- Changed `GameObjectBehaviour` to `Actor` to make its intention more clear.
- Changed `GameObjectUtility` APIs to match the new `Actor` naming.
- Changed `EventListenerHandler<T>` to `EventRef<T>.Handler`.
- Changed `EventKeyRestrictions` to `EventKey.RestrictionOptions`.
- Changed `Despawn` being optional to be called during `Destroy`.
- Changed `Spawn` to be optional to be called during `Initialize`.
- Changed `Despawn` and `Spawn` to be protected and to not call `SetActive` anymore.
- Fixed unnecessary logging on `ScriptableSettings` when quitting.
- Removed already unused `IFixedUpdateListener`, `ILateUpdateListener` and `IUpdateListener`.

## [6.0.0] - 2022-04-04

- Changed `ManagedPool<T>.Shared` to a new class `SharedManagedPool`.
- Changed `ManagedPoolBase<T>` to `ManagedPool<T>`, alongside many API breaking changes.
- Fixed unloading of pools that are not fully loaded being inefficient.

## [5.1.0] - 2022-04-03

- Added documentation for public editor APIs.
- Added `Editor Startup Scene` feature through the project settings window.
- Added parameter to RemovePool to ask if the pool being removed should be unloaded too.
- Added FrameworkUtility.IsReloadingScripts to allow runtime code to check for that safely.
- Added menu item `Tools/Coimbra Framework/Assert Serializable Types`.
- Added menu item `Tools/Coimbra Framework/Reload Scripts`.
- Added menu item `Tools/Coimbra Framework/Preferences/Clear Console On Reload`.
- Added FrameworkEditorUtility.ClearConsoleWindow API.
- Changed `Settings Manager` dependency version to 2.0.1.
- Changed package display name to `Coimbra Framework`.
- Changed AddPool to allow non-unloaded pools, loading it in the process if needed.
- Fixed spamming of warnings when reloading scripts while having ScriptableSettings in the project.
- Removed `ProjectUserSettingsRepository` class as it is a built-in option in newer `Settings Manager` package.

## [5.0.0] - 2022-04-02

- Added EventRef and APIs on IEvenService to use it.
- Added call to Dispose inside OnDestroy in MonoBehaviourServiceBase.
- Added GameObjectBehaviour class that works similar to extending GameObject class itself.
- Added GameObjectPool and IPoolingService for pooling of GameObjects.
- Added IsPersistent API for GameObjects.
- Changed ScriptableSettings.Set to receive a bool parameter to define if an existing value can be overriden.
- Changed MonoBehaviourServiceBase to ServiceBase.
- Changed EventListenerHandler to use the new EventRef type.
- Changed GameObjectEventListenerComponent functionality to the new GameObjectBehaviour class.
- Changed back the default services implementations (Systems) to be public.

## [4.0.0] - 2022-03-25

- Added overload for IEventService.AddListener that accepts a list to add the generated event handle.
- Added RequireDerived attribute to ScriptableSettings.
- Added API to get the current create callback in ServiceLocator.
- Added TryGetValid API for the object extensions.
- Added RemoveAllListenersWithKey, RemoveAllListenersWithoutRestriction and ResetAllEventKeys APIs.
- Added API to reset the key from multiple events with the same key.
- Changed how the event keys work, adding the `EventKey` and `EventKeyRestrictions` types.
- Changed IEventService APIs to return a bool indicating if they actually worked.
- Changed EventRefHandler to EventListenerHandler.
- Changed overload for IEventService.Invoke that auto-creates a new instance to use generics instead of System.Type.
- Changed all methods in IEventService to require an IEvent.
- Changed GameObject DestroyEvent to Destroyed.
- Changed IEventService to no longer have a service key, removing also the only API that used it.
- Fixed typo in Fi**r**stPostLateUpdateEvent.
- Fixed id not being set when constructing a ServiceLocator.
- Fixed possible stack overflow in case of invoking an event inside its own invoke.
- Fixed indirectly disposing a Shared service when allowing fallback to Shared and calling ServiceLocator.Set.
- Fixed build errors on ScriptableSettings.
- Removed overload for IEventService.AddListener that accepts non-by-ref methods.
- Removed support for non-interface services.
- Removed SetDefaultCreateCallback API as its usage was unclear and caused confusion.

## [3.1.1] - 2022-03-24

- Added option make ScriptableSettings non-preloaded.
- Fixed ScriptableSettings not working for ScriptableObjects that had their base class changed after being created.
- Fixed lack of validation for ScriptableSettings that requires preloading.

## [3.1.0] - 2022-03-23

- Added a new event for each PlayerLoopTiming available.
- Added Clear overload for List<T> that also receives a new min capacity.
- Added base IEvent interface to prepare for 2021 LTS update to use default interface methods.
- Added base IPlayerLoopEvent interface to be the base of all PlayerLoop-related events.
- Changed Disposable to be a ref struct.
- Fixed ApplicationSystem being created before BeforeSceneLoad entry point.
- Fixed major performance regression in EventSystem.

## [3.0.1] - 2022-03-22

- Added optional parameter to dispose the previously set system when setting a new one.
- Fixed ServiceLocator not setting the OwningLocator properly.
- Fixed EventSystem not allowing to remove listeners during the invoke process. The remove call will not take effect for the invoke currently running.

## [3.0.0] - 2022-03-21

- Changed the ScriptableSettings APIs to require ScriptableSettings objects.
- Fixed code stripping issues with the services.
- Removed obsolete APIs:
    - Removed IFixedUpdateService in favor of FixedUpdateEvent.
    - Removed ILateUpdateService in favor of LateUpdateEvent.
    - Removed IUpdateService in favor of UpdateEvent.
- Removed Coimbra.Systems, moving all systems to Coimbra assembly as internal classes.

## [2.2.0] - 2022-03-19

- Added ScriptableSettings to allow easy sharing of ScriptableObject data with non-MonoBehaviour objects.
- Added API documentation for GameObjectUtility.
- Added support for PreloadedAsset inside the editor, including cleanup of missing/null references.
- Added missing assert on non-generic APIs of ServiceLocator.
- Changed GameObjectEventListener to be internal.
- Changed GameObjects to send the GameObject instead of the GameObjectEventListener.
- Fixed IApplicationService missing RequireImplementors attribute.

## [2.1.0] - 2022-03-18

- Added FixedUpdateEvent, LateUpdateEvent, and UpdateServiceEvent to listen to the respective Unity loop callback.
- Added OwningLocator field to IService locator to be able to create services composed of multiple services easily.
- Added overloads that accepts event data by reference in the IEventService to avoid large struct copies.
- Added missing readonly keyword on built-in events.
- Changed ServiceLocator to be Serializable to be able to see which ServiceLocator a MonoBehaviour system belongs to.
- Deprecated IFixedUpdateService, ILateUpdateService, and IUpdateService in favor of new built-in events.

## [2.0.0] - 2022-03-16

- Added IService and enforce its usage in ServiceLocator APIs
- Added IFixedUpdateService, ILateUpdateService, and IUpdateService to register on the respective Unity loop callback.
- Added object.GetValid() and object.IsValid() APIs to make safer to work with abstractions and Unity Objects.
- Added ApplicationFocusEvent, ApplicationPauseEvent, and ApplicationQuitEvent. They are meant to be listened through the EventService.
- Added proper hide flags for all default implementations.
- Added GetValid call for ManagedField.Value and ServiceLocator.Get APIs to make those compatible with `?.` operator.
- Changed folder structure to group similar types together.
- Changed all services to implement the Dispose method to allow per-service cleanup.
- Fixed ServiceLocator not being compatible with Enter Play Mode Options.
- Refactored IEventService and its default implementation:
    - Added HasAnyListeners and HasListener APIs.
    - Added Invoke API that accepts a type and constructs a default event data to be used.
    - Added option to ignore the exception when the event key doesn't match.
    - Changed AddListener API to return an EventHandle.
    - Changed RemoveListener to use an EventHandle, allowing to remove anonymous method too.
- Refactored ITimerService and its default implementation:
    - Changed ITimerService to not have ref params to simplify its usage.
    - Changed TimerHandle to use System.Guid to better ensure uniqueness and made it a readonly struct.
    - Fixed TimerService not working for concurrent timers and added test cases to ensure minimum stability.
- Removed IApplicationService and its default implementation.
- Renamed all default implementations to `System` instead of `Service`.
- Renamed Coimbra.Services to Coimbra.Systems.

## [1.1.0] - 2022-03-08

- Added per-instance option to fallback to the ServiceLocator.Shared on non-shared instances. Enabled by default.
- Added runtime check to ensure that ServiceLocator is only used with interface types. Enabled by default, but can be disabled per-instance (except on the Shared instance).

## [1.0.0] - 2022-01-11

- Initial release
