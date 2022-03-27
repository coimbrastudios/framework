using System.Collections.Generic;
using UnityEngine;

namespace Coimbra
{
    internal sealed class PoolingSystem : IPoolingService
    {
        public ServiceLocator OwningLocator { get; set; }

        public bool AddPool(GameObjectPool pool, bool isPersistent)
        {
            return false;
        }

        public bool ContainsPool(GameObjectPool pool)
        {
            return false;
        }

        public GameObjectPool.DespawnResult Despawn(GameObject instance)
        {
            return GameObjectPool.DespawnResult.Despawned;
        }

        public GameObjectPool.DespawnResult Despawn(GameObjectBehaviour instance)
        {
            return GameObjectPool.DespawnResult.Despawned;
        }

        public void Dispose() { }

        public GameObjectPool[] GetAllPools()
        {
            return new GameObjectPool[]
                { };
        }

        public int GetAllPools(List<GameObjectPool> appendResults)
        {
            return 0;
        }

        public bool RemovePool(GameObjectPool pool)
        {
            return false;
        }

        public bool SetPoolPersistent(GameObjectPool pool, bool isPersistent)
        {
            return false;
        }

        public GameObject Spawn(GameObject prefab, Transform parent = null, bool spawnInWorldSpace = false)
        {
            return null;
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Vector3 rotation, Transform parent = null)
        {
            return null;
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            return null;
        }

        public T Spawn<T>(T prefab, Transform parent = null, bool spawnInWorldSpace = false)
            where T : GameObjectBehaviour
        {
            return null;
        }

        public T Spawn<T>(T prefab, Vector3 position, Vector3 rotation, Transform parent = null)
            where T : GameObjectBehaviour
        {
            return null;
        }

        public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null)
            where T : GameObjectBehaviour
        {
            return null;
        }
    }
}
