using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Coimbra.Tests
{
    [TestFixture]
    [TestOf(typeof(GameObjectBehaviour))]
    public class GameObjectBehaviourTests
    {
        [Test]
        public void GivenActivePrefab_WhenInstantiated_ThenCachesAreValid()
        {
            GameObjectBehaviour prefab = new GameObject().AddComponent<GameObjectBehaviour>();
            GameObjectBehaviour instance = Object.Instantiate(prefab);
            Assert.That(instance.CachedGameObject != null);
            Assert.That(instance.CachedTransform != null);
        }

        [Test]
        public void GivenInactivePrefab_WhenInstantiated_ThenCachesAreInvalid()
        {
            GameObject prefab = new GameObject();
            prefab.SetActive(false);

            GameObjectBehaviour prefabBehaviour = prefab.AddComponent<GameObjectBehaviour>();
            GameObjectBehaviour instance = Object.Instantiate(prefabBehaviour);
            Assert.That(instance.CachedGameObject == null);
            Assert.That(instance.CachedTransform == null);
        }

        [Test]
        public void GivenActiveInstance_WhenDisabled_ThenActiveStateChangedTriggers_AndStateIsFalse()
        {
            const string logFormat = "OnActivateStateChanged.state = {0}";
            GameObjectBehaviour prefab = new GameObject().AddComponent<GameObjectBehaviour>();
            GameObjectBehaviour instance = Object.Instantiate(prefab);
            instance.OnActiveStateChanged += delegate(GameObject sender, bool state)
            {
                Debug.LogFormat(logFormat, state);
            };

            instance.CachedGameObject.SetActive(false);
            LogAssert.Expect(LogType.Log, string.Format(logFormat, false));
        }

        [Test]
        public void GivenInactiveInstance_AndWasActive_WhenEnabled_ThenActivateStateChangedTriggers_AndStateIsTrue()
        {
            const string logFormat = "OnActivateStateChanged.state = {0}";
            GameObjectBehaviour prefab = new GameObject().AddComponent<GameObjectBehaviour>();
            GameObjectBehaviour instance = Object.Instantiate(prefab);
            instance.CachedGameObject.SetActive(false);
            instance.OnActiveStateChanged += delegate(GameObject sender, bool state)
            {
                Debug.LogFormat(logFormat, state);
            };

            instance.CachedGameObject.SetActive(true);
            LogAssert.Expect(LogType.Log, string.Format(logFormat, true));
        }

        [Test]
        public void GivenActivePrefab_AndHasPool_WhenInstantiated_ThenIsPooled()
        {
            GameObjectBehaviour prefab = new GameObject().AddComponent<GameObjectBehaviour>();
            prefab.Pool = new GameObject().AddComponent<GameObjectPool>();

            GameObjectBehaviour instance = Object.Instantiate(prefab);
            instance.Instantiate();
            Assert.That(instance.IsPooled);
        }

        [UnityTest]
        public IEnumerator GivenActiveInstance_WhenDestroyedByDestroyCall_ThenResultIsExplicitCall()
        {
            const string logFormat = "OnDestroyed.reason = {0}";
            GameObjectBehaviour instance = new GameObject().AddComponent<GameObjectBehaviour>();
            instance.OnDestroyed += delegate(GameObject sender, DestroyReason reason)
            {
                Debug.LogFormat(logFormat, reason);
            };

            LogAssert.Expect(LogType.Log, string.Format(logFormat, DestroyReason.ExplicitCall));
            Object.Destroy(instance.gameObject);

            yield return null;
        }
    }
}
