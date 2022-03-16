using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.TestTools;

namespace Coimbra.Tests
{
    [TestFixture]
    [TestOf(typeof(IEventService))]
    public class EventServiceTests
    {
        private struct TestEvent { }

        private IEventService _eventService;

        [SetUp]
        public void SetUp()
        {
            ServiceLocator.Shared.TryGet(out _eventService);
            Assert.IsNotNull(_eventService);
        }

        [TearDown]
        public void TearDown()
        {
            ServiceLocator.Shared.Dispose();
        }

        [Test]
        public void AddListener_Single()
        {
            const string log = nameof(log);
            _eventService.AddListener(delegate(object sender, TestEvent testEvent)
            {
                Debug.Log(log);
            });

            LogAssert.Expect(LogType.Log, log);
            _eventService.Invoke(this, new TestEvent());
        }

        [Test]
        public void AddListener_Multiple()
        {
            const string logA = nameof(logA);
            const string logB = nameof(logB);
            _eventService.AddListener(delegate(object sender, TestEvent testEvent)
            {
                Debug.Log(logA);
            });

            _eventService.AddListener(delegate(object sender, TestEvent testEvent)
            {
                Debug.Log(logB);
            });

            LogAssert.Expect(LogType.Log, logA);
            LogAssert.Expect(LogType.Log, logB);
            _eventService.Invoke(this, new TestEvent());
        }

        [Test]
        public void RemoveListener_Empty()
        {
            _eventService.RemoveListener(EventHandle.Create(typeof(TestEvent)));
            _eventService.Invoke(this, new TestEvent());
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void RemoveListener_Single()
        {
            const string log = nameof(log);

            static void callback(object sender, TestEvent testEvent)
            {
                Debug.Log(log);
            }

            EventHandle handle = _eventService.AddListener<TestEvent>(callback);
            _eventService.RemoveListener(handle);
            _eventService.Invoke(this, new TestEvent());
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void RemoveListener_Multiple()
        {
            const string logA = nameof(logA);
            const string logB = nameof(logB);

            static void callbackA(object sender, TestEvent testEvent)
            {
                Debug.Log(logA);
            }

            static void callbackB(object sender, TestEvent testEvent)
            {
                Debug.Log(logB);
            }

            EventHandle handle = _eventService.AddListener<TestEvent>(callbackA);
            _eventService.AddListener<TestEvent>(callbackB);
            _eventService.RemoveListener(handle);

            LogAssert.Expect(LogType.Log, logB);
            _eventService.Invoke(this, new TestEvent());
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void RemoveAllListeners_Empty()
        {
            _eventService.RemoveAllListeners<TestEvent>();
            _eventService.Invoke(this, new TestEvent());
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void RemoveAllListeners_Multiple()
        {
            const string logA = nameof(logA);
            const string logB = nameof(logB);
            _eventService.AddListener(delegate(object sender, TestEvent testEvent)
            {
                Debug.Log(logA);
            });

            _eventService.AddListener(delegate(object sender, TestEvent testEvent)
            {
                Debug.Log(logB);
            });

            _eventService.RemoveAllListeners<TestEvent>();
            _eventService.Invoke(this, new TestEvent());
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void Invoke_ThrowsInvalidOperationException_AfterSetEventKey()
        {
            const string log = nameof(log);
            _eventService.SetEventKey<TestEvent>(new object());
            _eventService.AddListener(delegate(object sender, TestEvent testEvent)
            {
                Debug.Log(log);
            });

            Assert.Throws<InvalidOperationException>(delegate
            {
                _eventService.Invoke(this, new TestEvent());
            });

            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void Invoke_AfterSetEventKey_WithCorrectKey()
        {
            const string log = nameof(log);
            object eventKey = new object();
            _eventService.SetEventKey<TestEvent>(eventKey);
            _eventService.AddListener(delegate(object sender, TestEvent testEvent)
            {
                Debug.Log(log);
            });

            LogAssert.Expect(LogType.Log, log);
            Assert.DoesNotThrow(delegate
            {
                _eventService.Invoke(this, new TestEvent(), eventKey);
            });
        }

        [Test]
        public void Invoke_AfterSetEventKey_AndResetEventKey()
        {
            const string log = nameof(log);
            _eventService.AddListener(delegate(object sender, TestEvent testEvent)
            {
                Debug.Log(log);
            });

            LogAssert.Expect(LogType.Log, log);
            object eventKey = new object();
            _eventService.SetEventKey<TestEvent>(eventKey);
            _eventService.ResetEventKey<TestEvent>(eventKey);
            Assert.DoesNotThrow(delegate
            {
                _eventService.Invoke(this, new TestEvent());
            });
        }

        [Test]
        public void ResetEventKey_ThrowsInvalidOperationException_WithWrongKey()
        {
            Assert.DoesNotThrow(delegate
            {
                _eventService.SetEventKey<TestEvent>(new object());
            });

            Assert.Throws<InvalidOperationException>(delegate
            {
                _eventService.ResetEventKey<TestEvent>(new object());
            });
        }

        [Test]
        public void SetEventKey_ThrowsInvalidOperationException_AfterSetEventKey()
        {
            Assert.DoesNotThrow(delegate
            {
                _eventService.SetEventKey<TestEvent>(new object());
            });

            Assert.Throws<InvalidOperationException>(delegate
            {
                _eventService.SetEventKey<TestEvent>(new object());
            });
        }
    }
}
