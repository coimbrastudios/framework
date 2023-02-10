# [Coimbra Framework](Index.md): Event Service

Strongly-typed event system that takes full advantage of `Source Generators` and `Roslyn Analyzers`.

The [IEventService] is designed to be a flexible strongly-typed event system, mostly focused in global events (common C# events are still recommended for events at [Actor]-level in most cases).

Designed with performance and debuggability in mind, with each new event having its own APIs generated that wraps the actual call to the equivalent [IEventService] API.
This allows anyone to find usages of a single event type and only see the relevant results.

For maximum performance, use structs to implement your events.
You can use class if really needed, but reusing the event instance is recommended to avoid generating garbage for each invocation.

You can check its usage details in the [IEvent] and [IEventService] APIs.
Check also the `Project Settings/Coimbra Framework/Event Settings` window for additional options and the `Window/Service Locator` window for runtime debug information (if using the [ServiceLocator]).

## Implementing Events

To implement a new event you only need to:

1. (Optional) Define your own [IEvent] interface.
2. Implement either [IEvent] or your own defined interface in a struct (recommended) or sealed class.
3. Ensure that the struct or class is partial.

> Abstract classes can be used, they will be incompatible with the [IEventService] APIs and won't have generated methods.
> They sole purpose would be to reuse code across different sealed events implementations.

## Default Events

Some [default services](ServiceLocator.md#default-services) were designed to invoke commonly needed events:

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

> You might notice those events have a custom [IEvent] definition interface, this is to take full advantage from both [AllowEventServiceUsageForAttribute] and the generic APIs at [IEventService] that are allowed by this attribute.

[Actor]:<Actor.md>

[ServiceLocator]:<ServiceLocator>

[AllowEventServiceUsageForAttribute]:<../Coimbra.Services.Events/AllowEventServiceUsageForAttribute.cs>

[IEvent]:<../Coimbra.Services.Events/IEvent.cs>

[IEventService]:<../Coimbra.Services.Events/IEventService.cs>
