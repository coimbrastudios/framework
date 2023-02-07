# Coimbra Framework: Pooling

    Under construction

Offers a thread-safe solution for pooling managed object and an `Addressables`-compatible solution for pooling `GameObject`.

## [GameObjectPool]

Specialized pooling implementation for `GameObject` with auto resizing functionality and support for `Addressables`.

## [ManagedPool<T>]

Comes with a few `SharedManagedPool` implementations that can be inspected at `Window/Coimbra Framework/Shared Managed Pools`:
- 
- `DictionaryPool`: clears the `Dictionary`.
- `GUIContentPool`: clears the `GUIContet`.
- `HashSetPool`: : clears the `HashSet`.
- `ListPool`: clears the `List`.
- `ManagedPool`: allows custom clearing through the `IManagedPoolHandler` interface.
- `QueuePool`: clears the `Queue`.
- `StackPool`: clears the `Stack`.
- `StringBuilderPool`: clears the `StringBuilder`.

## [PoolService]

Have same overloads as `Object.Instantiate` to make easier to migrate existing code.

[GameObjectPool]:<../Coimbra/GameObjectPool.cs>
[ManagedPool<T>]:<../Coimbra/ManagedPool`1.cs>
[PoolService]:<../Coimbra/GameObjectPool.cs>
