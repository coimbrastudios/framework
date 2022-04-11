# Changelog

## [8.0.0] - 2022--

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
- Added `ScriptableSettingsProvider` for easily displaying any `ScriptableSettings` in the project settings.
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
