# Changelog

## [2.0.0] - 2022-03-16

- Added IFixedUpdateService, ILateUpdateService, and IUpdateService to register on the respective Unity loop callback.
- Added object.GetValid() and object.IsValid() APIs to make safer to work with abstractions and Unity Objects.
- Added ApplicationFocusEvent, ApplicationPauseEvent, and ApplicationQuitEvent. They are meant to be listened through the EventService.
- Added proper hide flags for all default implementations.
- Added GetValid call for ManagedField.Value and ServiceLocator.Get APIs to make those compatible with `?.` operator.
- Changed folder structure to group similar types together.
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

## [1.1.0] - 2022-03-08

- Added per-instance option to fallback to the ServiceLocator.Shared on non-shared instances. Enabled by default.
- Added runtime check to ensure that ServiceLocator is only used with interface types. Enabled by default, but can be disabled per-instance (except on the Shared instance).

## [1.0.0] - 2022-01-11

- Initial release
