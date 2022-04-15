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
            _timerService = TimerSystem.Create();
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
            _timerService.StartTimer(delegate
            {
                Assert.That(id == 1);
                id++;
            }, 0.1f);

            _timerService.StartTimer(delegate
            {
                Assert.That(id == 0);
                id++;
            }, 0.05f);

            _timerService.StartTimer(delegate
            {
                Assert.That(id == 2);
                id++;
            }, 0.15f);

            yield return new WaitForSeconds(0.2f);

            Assert.That(id == 3);
        }

        [UnityTest]
        public IEnumerator TimerLoopsCorrectly()
        {
            int id = 0;
            TimerHandle timerHandle = _timerService.StartTimer(delegate
            {
                id++;
            }, 0, 0.01f, 5);

            yield return new WaitForSeconds(0.1f);

            Assert.That(id == 5);
            Assert.That(!_timerService.IsTimerActive(in timerHandle));
        }

        [UnityTest]
        public IEnumerator TimerStopsCorrectly()
        {
            int id = 0;
            TimerHandle targetTimerHandle = _timerService.StartTimer(delegate
            {
                id++;
            }, 0.15f);

            _timerService.StartTimer(delegate
            {
                _timerService.StopTimer(in targetTimerHandle);
            }, 0.05f);

            yield return new WaitForSeconds(0.1f);

            Assert.That(id == 0);
        }
    }
}
