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
            GameObject prefab = new GameObject();
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
            instance.OnActiveStateChanged += delegate(Actor sender, bool state)
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
            instance.OnActiveStateChanged += delegate(Actor sender, bool state)
            {
                Debug.LogFormat(logFormat, state);
            };

            instance.CachedGameObject.SetActive(true);
            LogAssert.Expect(LogType.Log, string.Format(logFormat, true));
        }

        [Test]
        public void GivenActivePrefab_AndHasPool_WhenInstantiated_ThenIsPooled()
        {
            Actor prefab = new GameObject().AddComponent<Actor>();
            prefab.Pool = new GameObject().AddComponent<GameObjectPool>();

            Actor instance = Object.Instantiate(prefab);
            Assert.That(instance.IsPooled, Is.True);
        }

        [UnityTest]
        public IEnumerator GivenActiveInstance_WhenDestroyedByDestroyCall_ThenResultIsExplicitCall()
        {
            const string logFormat = "OnDestroyed.reason = {0}";
            Actor instance = new GameObject().AddComponent<Actor>();
            instance.OnDestroyed += delegate(Actor sender, Actor.DestroyReason reason)
            {
                Debug.LogFormat(logFormat, reason);
            };

            LogAssert.Expect(LogType.Log, string.Format(logFormat, Actor.DestroyReason.ExplicitCall));
            Object.Destroy(instance.gameObject);

            yield return null;
        }
    }
}
