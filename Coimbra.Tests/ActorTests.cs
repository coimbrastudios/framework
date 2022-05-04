using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Coimbra.Tests
{
    [TestFixture]
    [TestOf(typeof(Actor))]
    public class GameObjectBehaviourTests
    {
        [Test]
        public void GivenActivePrefab_WhenInstantiated_ThenCachesAreValid()
        {
            Actor prefab = new GameObject().AddComponent<Actor>();
            Actor instance = Object.Instantiate(prefab);
            Assert.That(instance.CachedGameObject, Is.Not.Null);
            Assert.That(instance.CachedTransform, Is.Not.Null);
        }

        [Test]
        public void GivenInactivePrefab_WhenInstantiated_ThenCachesAreInvalid()
        {
            GameObject prefab = new();
            prefab.SetActive(false);

            Actor prefabBehaviour = prefab.AddComponent<Actor>();
            Actor instance = Object.Instantiate(prefabBehaviour);
            Assert.That(instance.CachedGameObject, Is.Null);
            Assert.That(instance.CachedTransform, Is.Null);
        }

        [Test]
        public void GivenActiveInstance_WhenDisabled_ThenActiveStateChangedTriggers_AndStateIsFalse()
        {
            const string logFormat = "OnActivateStateChanged.state = {0}";
            Actor prefab = new GameObject().AddComponent<Actor>();
            Actor instance = Object.Instantiate(prefab);
            instance.OnActiveStateChanged += delegate(Actor _, bool state)
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
            Actor prefab = new GameObject().AddComponent<Actor>();
            Actor instance = Object.Instantiate(prefab);
            instance.CachedGameObject.SetActive(false);
            instance.OnActiveStateChanged += delegate(Actor _, bool state)
            {
                Debug.LogFormat(logFormat, state);
            };

            instance.CachedGameObject.SetActive(true);
            LogAssert.Expect(LogType.Log, string.Format(logFormat, true));
        }

        [Test]
        public void GivenActivePrefab_AndHasPool_WhenInstantiated_ThenIsNotPooled()
        {
            Actor prefab = new GameObject().AddComponent<Actor>();
            GameObjectPool pool = new GameObject().AddComponent<GameObjectPool>();
            prefab.Initialize(pool, default);

            Actor instance = Object.Instantiate(prefab);
            Assert.That(instance.IsPooled, Is.False);
        }

        [UnityTest]
        public IEnumerator GivenActiveInstance_WhenDestroyedByDestroyCall_ThenResultIsExplicitCall()
        {
            const string logFormat = "OnDestroyed.reason = {0}";
            Actor instance = new GameObject().AddComponent<Actor>();
            instance.OnDestroying += delegate(Actor _, Actor.DestroyReason reason)
            {
                Debug.LogFormat(logFormat, reason);
            };

            LogAssert.Expect(LogType.Log, string.Format(logFormat, Actor.DestroyReason.ExplicitCall));
            Object.Destroy(instance.gameObject);

            yield return null;
        }
    }
}
