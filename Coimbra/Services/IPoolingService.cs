using System.Collections.Generic;
using UnityEngine;

namespace Coimbra
{
    public interface IPoolingService
    {
        /// <summary>
        /// Registers the specified pool on this service.
        /// </summary>
        /// <param name="pool">The pool to be registered.</param>
        /// <param name="isPersistent">Should the pool persist through scene changes?</param>
        /// <returns>True if the pool was registered.</returns>
        bool AddPool(GameObjectPool pool, bool isPersistent);

        /// <summary>
        /// Checks if a pool is registered on this service.
        /// </summary>
        /// <param name="pool">The pool to check.</param>
        /// <returns>True if the pool is currently registered.</returns>
        bool ContainsPool(GameObjectPool pool);

        /// <inheritdoc cref="GameObjectPool.Despawn(UnityEngine.GameObject)"/>
        GameObjectPool.DespawnResult Despawn(GameObject instance);

        /// <inheritdoc cref="GameObjectPool.Despawn(UnityEngine.GameObject)"/>
        GameObjectPool.DespawnResult Despawn(GameObjectBehaviour instance);

        /// <summary>
        /// Gets all pools currently registered on this service.
        /// </summary>
        /// <returns>A new array with all the registered pools.</returns>
        GameObjectPool[] GetAllPools();

        /// <summary>
        /// Gets all pools currently registered on this service.
        /// </summary>
        /// <param name="appendResults">The list to append the results.</param>
        /// <returns>The number of appended results.</returns>
        int GetAllPools(List<GameObjectPool> appendResults);

        /// <summary>
        /// Unregisters the specified pool from this service.
        /// </summary>
        /// <param name="pool">The pool to be unregistered.</param>
        /// <returns>True if the pool was unregistered</returns>
        bool RemovePool(GameObjectPool pool);

        /// <summary>
        /// Sets if the specified pool persist through scene changes.
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="isPersistent">Should the pool persist through scene changes?</param>
        /// <returns>True if the operation succeed.</returns>
        bool SetPoolPersistent(GameObjectPool pool, bool isPersistent);

        /// <inheritdoc cref="GameObjectPool.Spawn(Transform, bool)"/>
        GameObject Spawn(GameObject prefab, Transform parent = null, bool spawnInWorldSpace = false);

        /// <inheritdoc cref="GameObjectPool.Spawn(Vector3, Quaternion, Transform)"/>
        GameObject Spawn(GameObject prefab, Vector3 position, Vector3 rotation, Transform parent = null);

        /// <inheritdoc cref="GameObjectPool.Spawn(Vector3, Quaternion, Transform)"/>
        GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null);

        /// <inheritdoc cref="GameObjectPool.Spawn(Transform, bool)"/>
        T Spawn<T>(T prefab, Transform parent = null, bool spawnInWorldSpace = false)
            where T : GameObjectBehaviour;

        /// <inheritdoc cref="GameObjectPool.Spawn(Vector3, Quaternion, Transform)"/>
        T Spawn<T>(T prefab, Vector3 position, Vector3 rotation, Transform parent = null)
            where T : GameObjectBehaviour;

        /// <inheritdoc cref="GameObjectPool.Spawn(Vector3, Quaternion, Transform)"/>
        T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null)
            where T : GameObjectBehaviour;
    }
}
