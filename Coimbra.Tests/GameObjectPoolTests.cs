using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.TestTools;

namespace Coimbra.Tests
{
    [TestFixture]
    [TestOf(typeof(GameObjectPool))]
    public class GameObjectPoolTests
    {
#if COIMBRA_GAMEOBJECTPOOL_TESTS
        private const int Timeout = 1000;
        private const string PoolWithActivePrefabAddress = "Packages/com.coimbrastudios.core/Prefabs/GameObjectPool.prefab";
        private const string PoolWithInactivePrefabAddress = "Packages/com.coimbrastudios.core/Prefabs/GameObjectPool_InactivePrefab.prefab";

        private GameObjectPool _pool;
        private GameObjectPool _poolWithInactivePrefab;

        [UnitySetUp]
        [Timeout(Timeout)]
        public IEnumerator SetUp()
        {
            AsyncOperationHandle<GameObject> poolOperation = Addressables.InstantiateAsync(PoolWithActivePrefabAddress);
            AsyncOperationHandle<GameObject> poolWithInactivePrefabOperation = Addressables.InstantiateAsync(PoolWithInactivePrefabAddress);

            while (!poolOperation.IsDone || !poolWithInactivePrefabOperation.IsDone)
            {
                yield return null;
            }

            Assert.That(poolOperation.Result.TryGetComponent(out _pool), Is.True);
            Assert.That(poolWithInactivePrefabOperation.Result.TryGetComponent(out _poolWithInactivePrefab), Is.True);
        }

        [TearDown]
        public void TearDown()
        {
            _pool.Destroy();
            _poolWithInactivePrefab.Destroy();
        }

        [Test]
        public void GivenUnloadedPool_WhenDespawn_ThenFails()
        {
            Actor behaviour = new GameObject().AddComponent<Actor>();
            Assert.That(_pool.Despawn(behaviour), Is.EqualTo(GameObjectPool.DespawnResult.Aborted));
            Object.Destroy(behaviour.gameObject);
        }

        [Test]
        public void GivenUnloadedPool_WhenSpawn_ThenFails()
        {
            Assert.That(_pool.Spawn() == null);
        }

        [Test]
        public void GivenUnloadedPool_WhenLoad_ThenStateIsLoading()
        {
            _pool.OnStateChanged += delegate(GameObjectPool pool, GameObjectPool.State previous, GameObjectPool.State current)
            {
                Debug.Log(current);
            };

            _pool.LoadAsync().Forget();
            Assert.That(_pool.CurrentState, Is.EqualTo(GameObjectPool.State.Loading));
            LogAssert.Expect(LogType.Log, GameObjectPool.State.Loading.ToString());
        }

        [UnityTest]
        [Timeout(Timeout)]
        public IEnumerator GivenUnloadedPool_WhenLoad_ThenStateIsLoadedEventually()
        {
            _pool.OnStateChanged += delegate(GameObjectPool pool, GameObjectPool.State previous, GameObjectPool.State current)
            {
                Debug.Log(current);
            };

            _pool.LoadAsync().Forget();

            Assert.That(_pool.CurrentState, Is.EqualTo(GameObjectPool.State.Loading));

            yield return new WaitUntil(() => _pool.CurrentState == GameObjectPool.State.Loaded);

            LogAssert.Expect(LogType.Log, GameObjectPool.State.Loaded.ToString());
        }

        [Test]
        public void GivenUnloadedPool_WhenLoad_AndUnload_ThenStateIsUnloaded()
        {
            _pool.OnStateChanged += delegate(GameObjectPool pool, GameObjectPool.State previous, GameObjectPool.State current)
            {
                Debug.Log(current);
            };

            _pool.LoadAsync().Forget();
            Assert.That(_pool.Unload(), Is.True);
            Assert.That(_pool.CurrentState, Is.EqualTo(GameObjectPool.State.Unloaded));
            LogAssert.Expect(LogType.Log, GameObjectPool.State.Unloaded.ToString());
        }

        [UnityTest]
        [Timeout(Timeout)]
        public IEnumerator GivenLoadedPool_WhenUnload_ThenStateIsUnloaded()
        {
            yield return _pool.LoadAsync().ToCoroutine();

            Assert.That(_pool.CurrentState, Is.EqualTo(GameObjectPool.State.Loaded));
            Assert.That(_pool.Unload(), Is.True);
            Assert.That(_pool.CurrentState, Is.EqualTo(GameObjectPool.State.Unloaded));
        }

        [UnityTest]
        [Timeout(Timeout)]
        public IEnumerator GivenLoadedPool_WhenUnload_AndLoad_ThenStateIsLoadedEventually()
        {
            yield return _pool.LoadAsync().ToCoroutine();

            _pool.Unload();
            _pool.LoadAsync().Forget();

            Assert.That(_pool.CurrentState, Is.EqualTo(GameObjectPool.State.Loading));

            yield return new WaitUntil(() => _pool.CurrentState == GameObjectPool.State.Loaded);
        }

        [UnityTest]
        [Timeout(Timeout)]
        public IEnumerator GivenUnloadedPool_AndActivePrefab_WhenLoad_ThenInstantiateTriggersForEachInstance_AndInstancesAreValid()
        {
            _pool.MaxCapacity = 5;
            _pool.PreloadCount = 5;
            _pool.OnObjectInstantiated += delegate(GameObjectPool pool, Actor instance)
            {
                Debug.Log(pool);
                Assert.That(instance.IsSpawned, Is.False, nameof(instance.IsSpawned));
                AssertInstanceIsValid(pool, instance);
            };

            for (int i = 0; i < _pool.PreloadCount; i++)
            {
                LogAssert.Expect(LogType.Log, _pool.ToString());
            }

            yield return _pool.LoadAsync().ToCoroutine();
        }

        [UnityTest]
        [Timeout(Timeout)]
        public IEnumerator GivenUnloadedPool_AndInactivePrefab_WhenLoad_ThenInstantiateTriggersForEachInstance_AndInstancesAreValid()
        {
            _poolWithInactivePrefab.MaxCapacity = 5;
            _poolWithInactivePrefab.PreloadCount = 5;
            _poolWithInactivePrefab.OnObjectInstantiated += delegate(GameObjectPool pool, Actor instance)
            {
                Debug.Log(pool);
                Assert.That(instance.IsSpawned, Is.False, nameof(instance.IsSpawned));
                AssertInstanceIsValid(pool, instance);
            };

            for (int i = 0; i < _poolWithInactivePrefab.PreloadCount; i++)
            {
                LogAssert.Expect(LogType.Log, _poolWithInactivePrefab.ToString());
            }

            yield return _poolWithInactivePrefab.LoadAsync().ToCoroutine();
        }

        [UnityTest]
        [Timeout(Timeout)]
        public IEnumerator GivenLoadedPool_AndInactivePrefab_WhenSpawned_ThenInstanceIsSpawned()
        {
            _poolWithInactivePrefab.MaxCapacity = 1;
            _poolWithInactivePrefab.PreloadCount = 1;

            yield return _poolWithInactivePrefab.LoadAsync().ToCoroutine();

            Actor instance = _poolWithInactivePrefab.Spawn();
            Assert.That(instance.IsSpawned, Is.True);
        }

        [UnityTest]
        [Timeout(Timeout)]
        public IEnumerator GivenLoadedPool_AndPoolHasCapacity_WhenDespawned_ThenResultIsDespawned()
        {
            _pool.MaxCapacity = 1;
            _pool.PreloadCount = 1;

            yield return _pool.LoadAsync().ToCoroutine();

            Actor instance = _pool.Spawn();
            Assert.That(_pool.Despawn(instance), Is.EqualTo(GameObjectPool.DespawnResult.Despawned));
            Assert.That(instance.IsSpawned, Is.False);
        }

        [UnityTest]
        [Timeout(Timeout)]
        public IEnumerator GivenLoadedPool_AndPoolIsFull_WhenDespawned_ThenResultIsDestroyed()
        {
            _pool.PreloadCount = 1;
            _pool.MaxCapacity = 1;

            yield return _pool.LoadAsync().ToCoroutine();

            GameObject instance = new GameObject();
            Assert.That(_pool.Despawn(instance), Is.EqualTo(GameObjectPool.DespawnResult.Destroyed));

            yield return null;

            Assert.That(instance.IsValid(), Is.False);
        }

        [UnityTest]
        [Timeout(Timeout)]
        public IEnumerator GivenLoadedPool_AndSpawnedInstance_WhenDespawned_AndSpawned_ThenInstancesAreTheSame()
        {
            _pool.PreloadCount = 1;
            _pool.MaxCapacity = 1;

            yield return _pool.LoadAsync().ToCoroutine();

            Actor instanceA = _pool.Spawn();
            _pool.Despawn(instanceA);

            Actor instanceB = _pool.Spawn();
            Assert.That(instanceA, Is.EqualTo(instanceB));
            Assert.That(instanceA.IsSpawned, Is.True);
        }

        [UnityTest]
        [Timeout(Timeout)]
        public IEnumerator GivenLoadedPool_AndInactivePrefab_WhenDestroyed_ThenResultIsExplicitCall()
        {
            _poolWithInactivePrefab.MaxCapacity = 5;
            _poolWithInactivePrefab.PreloadCount = 5;
            _poolWithInactivePrefab.OnObjectInstantiated += delegate(GameObjectPool pool, Actor instance)
            {
                instance.OnDestroyed += delegate(Actor sender, DestroyReason reason)
                {
                    Debug.Log(reason);
                };
            };

            yield return _poolWithInactivePrefab.LoadAsync().ToCoroutine();

            _poolWithInactivePrefab.Unload();
            LogAssert.Expect(LogType.Log, DestroyReason.ExplicitCall.ToString());
        }

        [Test]
        public void GivenUnloadedPool_WhenSetPreloadCount_ThenIsNeverNegative()
        {
            _pool.PreloadCount = -1;
            Assert.That(_pool.PreloadCount, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public void GivenUnloadedPool_WhenSetMaxCapacity_ThenIsNeverNegative()
        {
            _pool.MaxCapacity = -1;
            Assert.That(_pool.MaxCapacity, Is.GreaterThanOrEqualTo(0));
        }

        [UnityTest]
        [Timeout(Timeout)]
        public IEnumerator GivenLoadedPool_WhenSetMaxCapacityToSmallerNumber_ThenAvailableInstanceCountAlsoChanges()
        {
            _pool.MaxCapacity = 5;
            _pool.PreloadCount = 5;

            yield return _pool.LoadAsync().ToCoroutine();

            Assert.That(_pool.AvailableInstancesCount, Is.EqualTo(5));
            _pool.MaxCapacity = 1;
            Assert.That(_pool.AvailableInstancesCount, Is.EqualTo(1));
        }

        private static void AssertInstanceIsValid(GameObjectPool pool, Actor instance)
        {
            Assert.That(instance.Pool, Is.EqualTo(pool), nameof(instance.Pool));
            Assert.That(instance.CachedTransform, Is.EqualTo(instance.transform), nameof(instance.CachedTransform));
            Assert.That(instance.CachedGameObject, Is.EqualTo(instance.gameObject), nameof(instance.CachedGameObject));
            Assert.That(instance.IsPooled, Is.True, nameof(instance.IsPooled));
        }
#endif
    }
}
