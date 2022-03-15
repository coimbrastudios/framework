# Changelog

## [1.2.0] - 2022-03-15

- Added IFixedUpdateService, ILateUpdateService, and IUpdateService to register on the respective Unity loop callback.
- Added object.GetValid() and object.IsValid() APIs to make safer to work with abstractions and Unity Objects.
- Changed TimerHandle to use System.Guid to better ensure uniqueness.
- Fixed TimerService not working for concurrent timers and added test cases to ensure minimum stability.

## [1.1.0] - 2022-03-08

- Added per-instance option to fallback to the ServiceLocator.Shared on non-shared instances. Enabled by default.
- Added runtime check to ensure that ServiceLocator is only used with interface types. Enabled by default, but can be disabled per-instance (except on the Shared instance).

## [1.0.0] - 2022-01-11

- Initial release
