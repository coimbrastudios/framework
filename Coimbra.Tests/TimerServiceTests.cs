using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Coimbra.Tests
{
    [TestFixture]
    [TestOf(typeof(ITimerService))]
    internal sealed class TimerServiceTests
    {
        private ITimerService _timerService;

        [SetUp]
        public void SetUp()
        {
            ServiceLocator.Shared.TryGet(out _timerService);
            Assert.IsNotNull(_timerService);
        }

        [TearDown]
        public void TearDown()
        {
            _timerService.StopAllTimers();
            _timerService = null;
            ServiceLocator.Shared.Set<ITimerService>(null);
        }

        [UnityTest]
        public IEnumerator TimersExecuteInOrder()
        {
            int id = 0;
            TimerHandle timerHandle = new TimerHandle();
            _timerService.StartTimer(ref timerHandle, delegate
            {
                Assert.That(id == 1);
                id++;
            }, 0.1f);

            timerHandle = new TimerHandle();
            _timerService.StartTimer(ref timerHandle, delegate
            {
                Assert.That(id == 0);
                id++;
            }, 0.05f);

            timerHandle = new TimerHandle();
            _timerService.StartTimer(ref timerHandle, delegate
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
            TimerHandle timerHandle = new TimerHandle();
            _timerService.StartTimer(ref timerHandle, delegate
            {
                id++;
            }, 0, 0.01f, 5);

            yield return new WaitForSeconds(0.1f);

            Assert.That(id == 5);
            Assert.That(!_timerService.IsTimerActive(ref timerHandle));
        }

        [UnityTest]
        public IEnumerator TimerStopsCorrectly()
        {
            int id = 0;
            TimerHandle sourceTimerHandle = new TimerHandle();
            TimerHandle targetTimerHandle = new TimerHandle();

            _timerService.StartTimer(ref sourceTimerHandle, delegate
            {
                // ReSharper disable once AccessToModifiedClosure
                _timerService.StopTimer(ref targetTimerHandle);
            }, 0.05f);

            _timerService.StartTimer(ref targetTimerHandle, delegate
            {
                id++;
            }, 0.15f);


            yield return new WaitForSeconds(0.1f);

            Assert.That(id == 0);
        }
    }
}
