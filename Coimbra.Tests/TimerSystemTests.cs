using Coimbra.Services.Timers;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Coimbra.Tests
{
    [TestFixture]
    [TestOf(typeof(TimerSystem))]
    public class TimerSystemTests
    {
        private ITimerService _timerService;

        [SetUp]
        public void SetUp()
        {
            _timerService = new GameObject(nameof(TimerSystem)).AsActor<TimerSystem>();
            Assert.IsNotNull(_timerService);
        }

        [TearDown]
        public void TearDown()
        {
            _timerService.Dispose();
            _timerService = null;
        }

        [UnityTest]
        public IEnumerator TimersExecuteInOrder()
        {
            int id = 0;

            void callback1()
            {
                Assert.That(id == 1);
                id++;
            }

            _timerService.StartTimer(callback1, 0.1f);

            void callback2()
            {
                Assert.That(id == 0);
                id++;
            }

            _timerService.StartTimer(callback2, 0.05f);

            void callback3()
            {
                Assert.That(id == 2);
                id++;
            }

            _timerService.StartTimer(callback3, 0.15f);

            yield return new WaitForSeconds(0.2f);

            Assert.That(id == 3);
        }

        [UnityTest]
        public IEnumerator TimerLoopsCorrectly()
        {
            int id = 0;

            void callback()
            {
                id++;
            }

            TimerHandle timerHandle = _timerService.StartTimer(callback, 0, 0.01f, 5);

            yield return new WaitForSeconds(0.1f);

            Assert.That(id == 5);
            Assert.That(!_timerService.IsTimerActive(in timerHandle));
        }

        [UnityTest]
        public IEnumerator TimerStopsCorrectly()
        {
            int id = 0;

            void callback1()
            {
                id++;
            }

            TimerHandle targetTimerHandle = _timerService.StartTimer(callback1, 0.15f);

            void callback2()
            {
                _timerService.StopTimer(in targetTimerHandle);
            }

            _timerService.StartTimer(callback2, 0.05f);

            yield return new WaitForSeconds(0.1f);

            Assert.That(id == 0);
        }
    }
}
