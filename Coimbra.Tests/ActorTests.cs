using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Coimbra.Tests
{
    [TestFixture]
    [TestOf(typeof(Actor))]
    public class ActorTests
    {
        [Test]
        public void GivenActiveInstance_WhenDisabled_ThenActiveStateChangedTriggers_AndStateIsFalse()
        {
            const string logFormat = "OnActivateStateChanged.state = {0}";
            Actor prefab = new GameObject().AsActor();
            Actor instance = Object.Instantiate(prefab);
            instance.Initialize();

            instance.OnActiveStateChanged += delegate(Actor _, bool state)
            {
                Debug.LogFormat(logFormat, state);
            };

            instance.GameObject.SetActive(false);
            LogAssert.Expect(LogType.Log, string.Format(logFormat, false));
        }

        [Test]
        public void GivenInactiveInstance_AndWasActive_WhenEnabled_ThenActivateStateChangedTriggers_AndStateIsTrue()
        {
            const string logFormat = "OnActivateStateChanged.state = {0}";
            Actor prefab = new GameObject().AsActor();
            Actor instance = Object.Instantiate(prefab);
            instance.Initialize();
            instance.GameObject.SetActive(false);

            instance.OnActiveStateChanged += delegate(Actor _, bool state)
            {
                Debug.LogFormat(logFormat, state);
            };

            instance.GameObject.SetActive(true);
            LogAssert.Expect(LogType.Log, string.Format(logFormat, true));
        }

        [Test]
        public void GivenActivePrefab_AndHasPool_WhenInstantiated_ThenIsNotPooled()
        {
            Actor prefab = new GameObject().AsActor();
            GameObjectPool pool = new GameObject().AsActor<GameObjectPool>();
            prefab.Initialize(pool, default);

            Actor instance = Object.Instantiate(prefab);
            instance.Initialize();
            Assert.That(instance.IsPooled, Is.False);
        }

        [UnityTest]
        public IEnumerator GivenActiveInstance_WhenDestroyedByDestroyCall_ThenResultIsExplicitCall()
        {
            const string logFormat = "OnDestroyed.reason = {0}";
            Actor instance = new GameObject().AsActor();

            instance.OnDestroying += delegate(Actor _, Actor.DestroyReason reason)
            {
                Debug.LogFormat(logFormat, reason);
            };

            LogAssert.Expect(LogType.Log, string.Format(logFormat, Actor.DestroyReason.ExplicitCall));
            instance.Destroy();

            yield return null;
        }
    }
}
