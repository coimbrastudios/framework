# Coimbra Framework: Event Service

    Under construction

Strongly-typed event system that takes full advantage of `Source Generators` and `Roslyn Analyzers`.

## Implementing Events

## Default Events

- [IApplicationStateEvent](../Coimbra.Services.ApplicationStateEvents/IApplicationStateEvent.cs)
  - [ApplicationFocusEvent](../Coimbra.Services.ApplicationStateEvents/ApplicationFocusEvent.cs)
  - [ApplicationPauseEvent](../Coimbra.Services.ApplicationStateEvents/ApplicationPauseEvent.cs)
  - [ApplicationQuitEvent](../Coimbra.Services.ApplicationStateEvents/ApplicationQuitEvent.cs)
- [IPlayerLoopEvent](../Coimbra.Services.PlayerLoopEvents/IPlayerLoopEvent.cs)
  - [FixedUpdateEvent](../Coimbra.Services.PlayerLoopEvents/Events/FixedUpdateEvent.cs)
  - [UpdateEvent](../Coimbra.Services.PlayerLoopEvents/Events/UpdateEvent.cs)
  - [LateUpdateEvent](../Coimbra.Services.PlayerLoopEvents/Events/LateUpdateEvent.cs)
  - [FirstInitializationEvent](../Coimbra.Services.PlayerLoopEvents/Events/FirstInitializationEvent.cs)
  - [LastInitializationEvent](../Coimbra.Services.PlayerLoopEvents/Events/LastInitializationEvent.cs)
  - [FirstEarlyUpdateEvent](../Coimbra.Services.PlayerLoopEvents/Events/FirstEarlyUpdateEvent.cs)
  - [LastEarlyUpdateEvent](../Coimbra.Services.PlayerLoopEvents/Events/LastEarlyUpdateEvent.cs)
  - [FirstFixedUpdateEvent](../Coimbra.Services.PlayerLoopEvents/Events/FirstFixedUpdateEvent.cs)
  - [LastFixedUpdateEvent](../Coimbra.Services.PlayerLoopEvents/Events/LastFixedUpdateEvent.cs)
  - [FirstPreUpdateEvent](../Coimbra.Services.PlayerLoopEvents/Events/FirstPreUpdateEvent.cs)
  - [LastPreUpdateEvent](../Coimbra.Services.PlayerLoopEvents/Events/LastPreUpdateEvent.cs)
  - [FirstUpdateEvent](../Coimbra.Services.PlayerLoopEvents/Events/FirstUpdateEvent.cs)
  - [LastUpdateEvent](../Coimbra.Services.PlayerLoopEvents/Events/LastUpdateEvent.cs)
  - [PreLateUpdateEvent](../Coimbra.Services.PlayerLoopEvents/Events/PreLateUpdateEvent.cs)
  - [FirstPostLateUpdateEvent](../Coimbra.Services.PlayerLoopEvents/Events/FirstPostLateUpdateEvent.cs)
  - [PostLateUpdateEvent](../Coimbra.Services.PlayerLoopEvents/Events/PostLateUpdateEvent.cs)
  - [LastPostLateUpdateEvent](../Coimbra.Services.PlayerLoopEvents/Events/LastPostLateUpdateEvent.cs)
  - [PreTimeUpdateEvent](../Coimbra.Services.PlayerLoopEvents/Events/PreTimeUpdateEvent.cs)
  - [PostTimeUpdateEvent](../Coimbra.Services.PlayerLoopEvents/Events/PostTimeUpdateEvent.cs)

## Using Events
