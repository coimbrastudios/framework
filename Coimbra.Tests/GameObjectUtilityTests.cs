using NUnit.Framework;
using UnityEngine;

namespace Coimbra.Tests
{
    [TestFixture]
    [TestOf(typeof(GameObjectUtility))]
    public class GameObjectUtilityTests
    {
        [Test]
        public void GivenGameObjectWithoutActor_WhenAsActor_ThenActorIsValid()
        {
            GameObject gameObject = new();
            Actor actor = gameObject.AsActor();
            Assert.That(actor, Is.Not.Null);
            Assert.That(actor.gameObject, Is.EqualTo(gameObject));
        }

        [Test]
        public void GivenGameObjectWithActor_WhenAsActor_ThenActorIsStillUnique()
        {
            GameObject gameObject = new("Test", typeof(Actor));
            Actor actor = gameObject.AsActor();
            Assert.That(actor.gameObject, Is.EqualTo(gameObject));
            Assert.That(gameObject.GetComponents<Actor>().Length, Is.EqualTo(1));
        }

        [Test]
        public void GivenGameObjectWithActor_AndItsTransform_WhenAsActorWithBoth_ThenBothResultsTheSame()
        {
            GameObject gameObject = new("Test", typeof(Actor));
            Transform transform = gameObject.transform;
            Assert.That(gameObject.AsActor(), Is.EqualTo(transform.gameObject.AsActor()));
        }
    }
}
