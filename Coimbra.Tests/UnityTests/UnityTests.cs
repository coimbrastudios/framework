using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Coimbra.Tests.UnityTests
{
    [TestFixture]
    public class UnityTests
    {
        [Test]
        public void AssumesAwakeIsCalledWithoutDelay_WhenAdding()
        {
            LogAssert.Expect(LogType.Log, LogBehaviour.AwakeLog);
            LogAssert.Expect(LogType.Log, LogBehaviour.EnableLog);
            GameObject gameObject = new GameObject();
            gameObject.AddComponent<LogBehaviour>();
            LogAssert.NoUnexpectedReceived();
            Object.Destroy(gameObject);
        }

        [Test]
        public void AssumesAwakeIsCalledWithoutDelay_WhenConstructing()
        {
            LogAssert.Expect(LogType.Log, LogBehaviour.AwakeLog);
            LogAssert.Expect(LogType.Log, LogBehaviour.EnableLog);
            GameObject gameObject = new GameObject(nameof(AssumesAwakeIsCalledWithoutDelay_WhenConstructing), typeof(LogBehaviour));
            LogAssert.NoUnexpectedReceived();
            Object.Destroy(gameObject);
        }
    }
}
