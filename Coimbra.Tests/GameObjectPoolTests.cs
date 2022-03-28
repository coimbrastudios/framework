using NUnit.Framework;

namespace Coimbra.Tests
{
    [TestFixture]
    [TestOf(typeof(GameObjectPool))]
    public class GameObjectPoolTests
    {
        [Test]
        public void GivenInactivePrefab_WhenInstantiated_ThenIsPooled_AndCacheIsValid() { }

        [Test]
        public void GivenInactivePrefab_WhenDestroyed_ThenResultIsExplicitCall() { }
    }
}
