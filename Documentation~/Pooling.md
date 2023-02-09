# [Coimbra Framework](Index.md): Pooling

Offers a thread-safe solution for pooling managed object and an [Addressables]-compatible solution for pooling [GameObject].

## [GameObjectPool]

Specialized pooling implementation for [GameObject] with auto resizing functionality and support for [Addressables].

### Creating [GameObjectPool]

### Using [GameObjectPool]

## [ManagedPool<T>]

Comes with a few [SharedManagedPool] implementations that can be inspected at `Window/Coimbra Framework/Shared Managed Pools`:

## Creating [ManagedPool<T>]

## Using [ManagedPool<T>]

## Creating [SharedManagedPool]

## Using [SharedManagedPool]

## Default [SharedManagedPool]

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

[GameObjectPool]:<../Coimbra/GameObjectPool.cs>

[ManagedPool<T>]:<../Coimbra/ManagedPool`1.cs>

[PoolService]:<../Coimbra.Services.Pooling/IPoolService.cs>

[SharedManagedPool]:<../Coimbra/SharedManagedPoolAttribute.cs>

[DictionaryPool]:<../Coimbra/SharedManagedPools/DictionaryPool.cs>

[GUIContentPool]:<../Coimbra/SharedManagedPools/GUIContentPool.cs>

[HashSetPool]:<../Coimbra/SharedManagedPools/HashSetPool.cs>

[ListPool]:<../Coimbra/SharedManagedPools/ListPool.cs>

[ManagedPool]:<../Coimbra/SharedManagedPools/ManagedPool.cs>

[QueuePool]:<../Coimbra/SharedManagedPools/QueuePool.cs>

[StackPool]:<../Coimbra/SharedManagedPools/StackPool.cs>

[StringBuilderPool]:<../Coimbra/SharedManagedPools/StringBuilderPool.cs>

[Addressables]:<https://docs.unity3d.com/Manual/com.unity.addressables.html>

[GameObject]:<https://docs.unity3d.com/ScriptReference/GameObject.html>
[]:<https://docs.unity3d.com/ScriptReference/.html>
