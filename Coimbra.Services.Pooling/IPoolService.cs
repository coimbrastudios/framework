using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Scripting;

namespace Coimbra.Services.Pooling
{
    /// <summary>
    /// Pooling service for <see cref="GameObject"/>.
    /// </summary>
    [RequiredService]
    [RequireImplementors]
    public interface IPoolService : IService
    {
        /// <summary>
        /// Gets the amount of <see cref="GameObjectPool"/> still loading.
        /// </summary>
        int LoadingPoolCount { get; }

        /// <summary>
        /// Registers the specified <see cref="GameObjectPool"/> on this service.
        /// </summary>
        /// <param name="pool">The <see cref="GameObjectPool"/> to be registered.</param>
        /// <returns>True if the <see cref="GameObjectPool"/> was registered.</returns>
        bool AddPool(GameObjectPool pool);

        /// <summary>
        /// Checks if a <see cref="GameObjectPool"/> is registered on this service.
        /// </summary>
        /// <param name="pool">The <see cref="GameObjectPool"/> to check.</param>
        /// <returns>True if the <see cref="GameObjectPool"/> is currently registered.</returns>
        bool ContainsPool(GameObjectPool pool);

        /// <summary>
        /// Checks if a <see cref="GameObjectPool"/> with the specified prefab is registered on this service.
        /// </summary>
        /// <param name="prefab">The prefab to check for a matching <see cref="GameObjectPool"/>.</param>
        /// <returns>True if a <see cref="GameObjectPool"/> with the specified prefab is currently registered.</returns>
        bool ContainsPool(AssetReferenceT<GameObject> prefab);

        /// <summary>
        /// Gets all <see cref="GameObjectPool"/> currently registered on this service.
        /// </summary>
        /// <returns>A new array with all the registered poo<see cref="GameObjectPool"/>ls.</returns>
        GameObjectPool[] GetAllPools();

        /// <summary>
        /// Gets all <see cref="GameObjectPool"/> currently registered on this service.
        /// </summary>
        /// <param name="appendResults">The list to append the results.</param>
        /// <returns>The number of appended results.</returns>
        int GetAllPools(List<GameObjectPool> appendResults);

        /// <summary>
        /// Unregisters the specified <see cref="GameObjectPool"/> from this service.
        /// </summary>
        /// <param name="pool">The <see cref="GameObjectPool"/> to be unregistered.</param>
        /// <param name="unload">If true, it will also call <see cref="GameObjectPool.Unload"/>.</param>
        /// <returns>True if the <see cref="GameObjectPool"/> was unregistered.</returns>
        bool RemovePool(GameObjectPool pool, bool unload);

        /// <inheritdoc cref="GameObjectPool.Spawn(Transform, bool)"/>
        Actor Spawn(GameObject prefab, Transform parent = null, bool spawnInWorldSpace = false);

        /// <inheritdoc cref="GameObjectPool.Spawn(Vector3, Quaternion, Transform)"/>
        Actor Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null);

        /// <inheritdoc cref="GameObjectPool.Spawn(Vector3, Quaternion, Transform)"/>
        Actor Spawn(GameObject prefab, Vector3 position, Vector3 rotation, Transform parent = null);

        /// <inheritdoc cref="Spawn(GameObject, Transform, bool)"/>
        T Spawn<T>(T prefab, Transform parent = null, bool spawnInWorldSpace = false)
            where T : Actor;

        /// <inheritdoc cref="Spawn(GameObject, Vector3, Quaternion, Transform)"/>
        T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null)
            where T : Actor;

        /// <inheritdoc cref="Spawn(GameObject, Vector3, Quaternion, Transform)"/>
        T Spawn<T>(T prefab, Vector3 position, Vector3 rotation, Transform parent = null)
            where T : Actor;
    }
}
