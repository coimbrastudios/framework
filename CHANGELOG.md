# Changelog

## [5.0.0] - --

- Added EventRef and APIs on IEvenService to use it.
- Added call to Dispose inside OnDestroy in MonoBehaviourServiceBase.
- Added GameObjectBehaviour class that works similar to extending GameObject class itself.
- Added GameObjectPool and IPoolingService for pooling of GameObjects.
- Changed MonoBehaviourServiceBase to allow multiple on the same GameObject.
- Changed EventListenerHandler to use the new EventRef type.
- Changed GameObjectEventListenerComponent functionality to the new GameObjectBehaviour class.

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
