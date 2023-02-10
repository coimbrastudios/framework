# [Coimbra Framework](Index.md): Pooling

Offers a thread-safe solution for pooling managed object and an [Addressables]-compatible solution for pooling [GameObject].

## [GameObjectPool]

Specialized pooling implementation for [GameObject] with auto resizing functionality and support for [Addressables].

To use a [GameObjectPool] you first need to call `LoadAsync` and then you can start calling `Spawn`.
You can call `Unload` whenever the pool isn't needed anymore.

> The defaults for [GameObjectPool] are with usability in mind.
> Those values aren't that bad for performance, but if maximum performance is needed you will want to tweak those values.

You can check more details in the [GameObjectPool] APIs.

## [ManagedPool<T>]

A generic thread-safe solution for pooling any class.
To use it simply create a new instance using one of the provided constructors and listen the necessary events.

> Do **not** use the static `CreateShared` methods.
> Those are only meant to be used alongside [SharedManagedPool].

## [SharedManagedPool]

Those are static pools to be used from anywhere in your code.
Comes with a few default implementations that can be inspected at `Window/Coimbra Framework/Shared Managed Pools`:

- [DictionaryPool]
- [GUIContentPool]
- [HashSetPool]
- [ListPool]
- [QueuePool]
- [StackPool]
- [StringBuilderPool]

You can also create a new [SharedManagedPool] by following those steps:

1. Create a `static partial class` for your pool. Can't be generic.
2. If the pooled class is generic, you will need a nested `static class` (non-partial) with the necessary generic arguments.
3. Add a `static readonly` [ManagedPool<T>] field (either `internal` or `public`). If the pooled class is generic, this field should go inside the nested class created above.
4. Add [SharedManagedPool] attribute using the field name created above as the first argument. If the pooled class is generic, pass the nested class name as the second argument.
5. (Optional) Implement a static constructor to initialize the [ManagedPool<T>] field with the necessary events.

> It is actually a lot easier than it appears, check [StringBuilderPool] for a simple example.
> For more complex examples check [DictionaryPool], which requires 2 generic arguments, or the static [ManagedPool], which has specific constraints.

## [IManagedPoolHandler]

Instead of creating static pools, you can take advantage of the static [ManagedPool] and implement the pool logic at the object-level by implementing the [IManagedPoolHandler].

> It will require a parameterless constructor and to implement `OnPop`, `OnPush` and `OnDispose`.

## [PoolService]

Meant to be a complete substitute for `Object.Instantiate`, having `Spawn` methods with the exact same overloads as `Object.Instantiate` to make easier to migrate existing code.

When spawning something through the [PoolService] it will automatically check if there is a [GameObjectPool] for it or if should fallback to [Object.Instantiate] call.

Default persistent pools can be defined at `Project Settings/Coimbra Framework/Pool Setting`.
Pools can also be dynamically added and removed for the [PoolService] at runtime.

[DictionaryPool]:<../Coimbra/SharedManagedPools/DictionaryPool.cs>

[GameObjectPool]:<../Coimbra/GameObjectPool.cs>

[GUIContentPool]:<../Coimbra/SharedManagedPools/GUIContentPool.cs>

[HashSetPool]:<../Coimbra/SharedManagedPools/HashSetPool.cs>

[IManagedPoolHandler]:<../Coimbra/IManagedPoolHandler.cs>

[ListPool]:<../Coimbra/SharedManagedPools/ListPool.cs>

[ManagedPool]:<../Coimbra/SharedManagedPools/ManagedPool.cs>

[ManagedPool<T>]:<../Coimbra/ManagedPool`1.cs>

[PoolService]:<../Coimbra.Services.Pooling/IPoolService.cs>

[QueuePool]:<../Coimbra/SharedManagedPools/QueuePool.cs>

[SharedManagedPool]:<../Coimbra/SharedManagedPoolAttribute.cs>

[StackPool]:<../Coimbra/SharedManagedPools/StackPool.cs>

[StringBuilderPool]:<../Coimbra/SharedManagedPools/StringBuilderPool.cs>

[Addressables]:<https://docs.unity3d.com/Manual/com.unity.addressables.html>

[GameObject]:<https://docs.unity3d.com/ScriptReference/GameObject.html>
