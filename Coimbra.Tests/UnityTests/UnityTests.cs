using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Coimbra.Tests.UnityTests
{
    [TestFixture]
    public class UnityTests
    {
        [Test]
        public void GivenAddComponent_ThenAwakeIsCalledWithoutDelay()
        {
            LogAssert.Expect(LogType.Log, LogBehaviour.AwakeLog);
            LogAssert.Expect(LogType.Log, LogBehaviour.EnableLog);
            GameObject gameObject = new();
            gameObject.AddComponent<LogBehaviour>();
            LogAssert.NoUnexpectedReceived();
            gameObject.Destroy();
        }

        [Test]
        public void GivenNewGameObject_ThenAwakeIsCalledWithoutDelay()
        {
            LogAssert.Expect(LogType.Log, LogBehaviour.AwakeLog);
            LogAssert.Expect(LogType.Log, LogBehaviour.EnableLog);
            GameObject gameObject = new(nameof(GivenNewGameObject_ThenAwakeIsCalledWithoutDelay), typeof(LogBehaviour));
            LogAssert.NoUnexpectedReceived();
            gameObject.Destroy();
        }
    }
}
