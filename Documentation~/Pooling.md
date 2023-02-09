# [Coimbra Framework](Index.md): Pooling

    UNDER CONSTRUCTION

Offers a thread-safe solution for pooling managed object and an [Addressables]-compatible solution for pooling [GameObject].

## [GameObjectPool]

Specialized pooling implementation for [GameObject] with auto resizing functionality and support for [Addressables].

### Creating [GameObjectPool]

### Using [GameObjectPool]

## [ManagedPool<T>]

## Creating [ManagedPool<T>]

## Using [ManagedPool<T>]

## Creating [SharedManagedPool]

## Using [SharedManagedPool]

## [SharedManagedPool]

Comes with a few default implementations that can be inspected at `Window/Coimbra Framework/Shared Managed Pools`:

- [DictionaryPool]
- [GUIContentPool]
- [HashSetPool]
- [ListPool]
- [ManagedPool]
- [QueuePool]
- [StackPool]
- [StringBuilderPool]

## [PoolService]

Have same overloads as `Object.Instantiate` to make easier to migrate existing code.

[DictionaryPool]:<../Coimbra/SharedManagedPools/DictionaryPool.cs>

[GameObjectPool]:<../Coimbra/GameObjectPool.cs>

[GUIContentPool]:<../Coimbra/SharedManagedPools/GUIContentPool.cs>

[HashSetPool]:<../Coimbra/SharedManagedPools/HashSetPool.cs>

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
