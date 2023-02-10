# [Coimbra Framework](Index.md): [Actor]

Pretend you are inheriting from [GameObject] to make easier to design per-object behaviours.

It is common to need a main script for certain objects to glue all the attached components together, and this is the main purpose of the [Actor] class.
The main reason for that is because any individual [MonoBehaviour] can't have an implementation that is generic enough to work with all possible combination of different attached [Component].

[Actor] provides a custom lifecycle designed with the [GameObject] in mind, but due engine limitations you are required to both initialize and destroy it correctly.

> Remember: it is totally unsafe to remove an [Actor] from a [GameObject] without destroying both together.

## Initializing Actors

Due engine limitations, to be able to have control full control of the lifecycle of a [GameObject] the [Actor] requires its `Initialize` method to be called as early as possible after instantiation.

For scene-placed actors, nothing special is needed as everything is already handled internally in the package.
However, do have in mind that the initialization logic for any given scene will be:

1. Each [MonoBehaviour.Awake] callback.
2. Each [Actor] `OnInitialize` method followed immediately by their `OnSpawn` method.
3. Each [MonoBehaviour.Start] callback.

For runtime instantiation you need to call [Actor] `Initialize` method right after the [Object.Instantiate] call.

However, some APIs do that for you internally already:

- [GameObjectUtility] `AsActor`, `GetActor` and `IsActor` extension methods.
- [GameObjectPool] `Spawn` methods.
- [PoolSystem] `Spawn` methods.

When using any of those APIs you don't need to care about dealing with the `Initialize` method call.

> In case of doubt, just call `Initialize` as calling it after the initial call is always a no-op.

## Disposing Actors

Due engine limitations, we don't have way to now if a [GameObject] ever gets destroyed unless it was previously active at least once or if we inject our own custom logic between our code and the destruction logic itself.
For this reason this package offers a unified way to destroy any object safely:

- [Actor] `Dispose` instance method. When you destroy an [Actor] it destroys the [GameObject] too.
- [GameObject] `Dispose` extension method from [GameObjectUtility]. It will decide efficiently if it needs to call [Actor] `Dispose` or if it can destroy the [GameObject] directly.
- [Object] `Dispose` extension method from [ObjectUtility]. Whenever you call `Dispose` from an [Object] instance it will detect efficiently how to properly destroy it.

In short, **never** use [Object.Destroy] (i.e. `Destroy(_myObject)` or `Object.Destroy(_myObject)`).
Instead, **always** use either `Dispose(bool)`, `this.Dispose(bool)` or `_myObject.Dispose(bool)`.

> When you use `true` as the `bool` it will force an object destroy even if it actually could've returned to a [GameObjectPool].
> Usually using `false` is the preferred choice as when no [GameObjectPool] is linked it will already destroy the object instead.

## Implementing Actors

To implement a new actor you only need to:

- Inherit from [Actor].
- (Optional) Override `OnInitialize` for one-time initialization. This is called as soon that the objects gets instantiated.
- (Optional) Override `OnSpawn` for initialization logic that should happens everytime the object is actually spawned (i.e. from a [GameObjectPool]).
- (Optional) Override `OnDispose` for cleanup logic that should happens everytime the object is disposed (i.e. from a [GameObjectPool]).
- (Optional) Override `OnDestroyed` for one-time cleanup logic when the object is finally destroyed.

> If the [Actor] wasn't created by a [GameObjectPool] the `OnSpawn` will get called right after `OnInitialize`, and `OnDispose` will get called right before `OnDestroyed`.

## Implementing Actor Components

Alongside traditional [MonoBehaviour], you can also inherit from [ActorComponentBase] that has special hooks into the [Actor] initialization logic.
Those hooks are abstract methods, so hard to not notice those when implementing your own actor component. The [ActorComponentBase] also provides a property with the owning [Actor] to avoid boilerplate just for caching it.

> There is no benefit on inheriting from [ActorComponentBase] if you don't need those hooks and having an [Actor] attached makes no difference at all.
> Do use [MonoBehaviour] in those cases as [ActorComponentBase] has a [RequireComponentAttribute] with the [Actor] as argument, which might be annoying for components that doesn't actually require it.

## Default Actors and ActorComponents

This framework uses [Actor] in many places, you can check their implementation details to learn how to make the most from the provided APIs.

- [Actor]
    - [GameObjectPool]
    - [HierarchyFolder]
    - [ApplicationStateSystem]
    - [CoroutineSystem]
    - [PlayerLoopSystem]
    - [PoolSystem]
    - [TimerSystem]
- [ActorComponentBase]
    - [DebugOnlyComponent]
    - [Overlap2DListenerBase]
        - [ColliderOverlap2DListener]
        - [RigidbodyOverlap2DListener]
    - [TransformChangedListener]
    - [EventHandleTrackerComponent]

[GameObjectPool]:<Pooling.md#gameobjectpool>

[PoolSystem]:<Pooling.md#poolservice>

[Actor]:<../Coimbra/Actor.cs>

[ActorComponentBase]:<../Coimbra/ActorComponentBase.cs>

[ApplicationStateSystem]:<../Coimbra.Services.ApplicationStateEvents/ApplicationStateSystem.cs>

[ColliderOverlap2DListener]:<../Coimbra.Listeners/Physics2D/ColliderOverlap2DListener.cs>

[CoroutineSystem]:<../Coimbra.Services.Coroutines/CoroutineSystem.cs>

[DebugOnlyComponent]:<../Coimbra/DebugOnlyComponent.cs>

[EventHandleTrackerComponent]:<../Coimbra.Services.Events/EventHandleTrackerComponent.cs>

[GameObjectUtility]:<../Coimbra/Utilities/GameObjectUtility.cs>

[HierarchyFolder]:<../Coimbra/HierarchyFolder.cs>

[ObjectUtility]:<../Coimbra/Utilities/ObjectUtility.cs>

[Overlap2DListenerBase]:<../Coimbra.Listeners/Overlap2DListenerBase`1.cs>

[PlayerLoopSystem]:<../Coimbra.Services.PlayerLoopEvents/PlayerLoopSystem.cs>

[Pooling]:<Pooling.md>

[RigidbodyOverlap2DListener]:<../Coimbra.Listeners/Physics2D/RigidbodyOverlap2DListener.cs>

[TimerSystem]:<../Coimbra.Services.Timers/TimerSystem.cs>

[TransformChangedListener]:<../Coimbra.Listeners/Transform/TransformChangedListener.cs>

[Component]:<https://docs.unity3d.com/ScriptReference/Component.html>

[GameObject]:<https://docs.unity3d.com/ScriptReference/GameObject.html>

[MonoBehaviour]:<https://docs.unity3d.com/ScriptReference/MonoBehaviour.html>

[MonoBehaviour.Awake]:<https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html>

[MonoBehaviour.Start]:<https://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html>

[Object]:<https://docs.unity3d.com/ScriptReference/Object.html>

[Object.Destroy]:<https://docs.unity3d.com/ScriptReference/Object.Destroy.html>

[Object.Instantiate]:<https://docs.unity3d.com/ScriptReference/Object.Instantiate.html>

[RequireComponentAttribute]:<https://docs.unity3d.com/ScriptReference/RequireComponent.html>
