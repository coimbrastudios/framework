using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.TestTools;

namespace Coimbra.Services.Tests.Editor
{
    [TestFixture]
    [TestOf(typeof(EventService))]
    public class EventServiceTests
    {
        public struct TestEvent { }

        [Test]
        public void AddListener_Single()
        {
            const string log = nameof(log);
            IEventService eventService = new EventService();
            eventService.AddListener(delegate(object sender, TestEvent testEvent)
            {
                Debug.Log(log);
            });

            LogAssert.Expect(LogType.Log, log);
            eventService.Invoke(this, new TestEvent());
        }

        [Test]
        public void AddListener_Multiple()
        {
            const string logA = nameof(logA);
            const string logB = nameof(logB);

            IEventService eventService = new EventService();
            eventService.AddListener(delegate(object sender, TestEvent testEvent)
            {
                Debug.Log(logA);
            });

            eventService.AddListener(delegate(object sender, TestEvent testEvent)
            {
                Debug.Log(logB);
            });

            LogAssert.Expect(LogType.Log, logA);
            LogAssert.Expect(LogType.Log, logB);
            eventService.Invoke(this, new TestEvent());
        }

        [Test]
        public void RemoveListener_Empty()
        {
            const string log = nameof(log);
            IEventService eventService = new EventService();

            static void callback(object sender, TestEvent testEvent)
            {
                Debug.Log(log);
            }

            eventService.RemoveListener<TestEvent>(callback);
            eventService.Invoke(this, new TestEvent());
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void RemoveListener_Single()
        {
            const string log = nameof(log);
            IEventService eventService = new EventService();

            static void callback(object sender, TestEvent testEvent)
            {
                Debug.Log(log);
            }

            eventService.AddListener<TestEvent>(callback);
            eventService.RemoveListener<TestEvent>(callback);
            eventService.Invoke(this, new TestEvent());
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void RemoveListener_Multiple()
        {
            const string logA = nameof(logA);
            const string logB = nameof(logB);

            IEventService eventService = new EventService();

            static void callbackA(object sender, TestEvent testEvent)
            {
                Debug.Log(logA);
            }

            static void callbackB(object sender, TestEvent testEvent)
            {
                Debug.Log(logB);
            }

            eventService.AddListener<TestEvent>(callbackA);
            eventService.AddListener<TestEvent>(callbackB);
            eventService.RemoveListener<TestEvent>(callbackA);

            LogAssert.Expect(LogType.Log, logB);
            eventService.Invoke(this, new TestEvent());
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void RemoveAllListeners_Empty()
        {
            IEventService eventService = new EventService();
            eventService.RemoveAllListeners<TestEvent>();
            eventService.Invoke(this, new TestEvent());
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void RemoveAllListeners_Multiple()
        {
            const string logA = nameof(logA);
            const string logB = nameof(logB);

            IEventService eventService = new EventService();

            eventService.AddListener(delegate(object sender, TestEvent testEvent)
            {
                Debug.Log(logA);
            });

            eventService.AddListener(delegate(object sender, TestEvent testEvent)
            {
                Debug.Log(logB);
            });

            eventService.RemoveAllListeners<TestEvent>();
            eventService.Invoke(this, new TestEvent());
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void Invoke_ThrowsInvalidOperationException_AfterSetEventKey()
        {
            const string log = nameof(log);
            IEventService eventService = new EventService();
            eventService.SetEventKey<TestEvent>(new object());
            eventService.AddListener(delegate(object sender, TestEvent testEvent)
            {
                Debug.Log(log);
            });

            Assert.Throws<InvalidOperationException>(delegate
            {
                eventService.Invoke(this, new TestEvent());
            });

            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void Invoke_AfterSetEventKey_WithCorrectKey()
        {
            const string log = nameof(log);
            object eventKey = new object();
            IEventService eventService = new EventService();
            eventService.SetEventKey<TestEvent>(eventKey);
            eventService.AddListener(delegate(object sender, TestEvent testEvent)
            {
                Debug.Log(log);
            });

            LogAssert.Expect(LogType.Log, log);
            Assert.DoesNotThrow(delegate
            {
                eventService.Invoke(this, new TestEvent(), eventKey);
            });
        }

        [Test]
        public void Invoke_AfterSetEventKey_AndResetEventKey()
        {
            const string log = nameof(log);
            IEventService eventService = new EventService();
            eventService.AddListener(delegate(object sender, TestEvent testEvent)
            {
                Debug.Log(log);
            });

            LogAssert.Expect(LogType.Log, log);
            object eventKey = new object();
            eventService.SetEventKey<TestEvent>(eventKey);
            eventService.ResetEventKey<TestEvent>(eventKey);
            Assert.DoesNotThrow(delegate
            {
                eventService.Invoke(this, new TestEvent());
            });
        }

        [Test]
        public void ResetEventKey_ThrowsInvalidOperationException_WithWrongKey()
        {
            IEventService eventService = new EventService();
            Assert.DoesNotThrow(delegate
            {
                eventService.SetEventKey<TestEvent>(new object());
            });

            Assert.Throws<InvalidOperationException>(delegate
            {
                eventService.ResetEventKey<TestEvent>(new object());
            });
        }

        [Test]
        public void SetEventKey_ThrowsInvalidOperationException_AfterSetEventKey()
        {
            IEventService eventService = new EventService();
            Assert.DoesNotThrow(delegate
            {
                eventService.SetEventKey<TestEvent>(new object());
            });

            Assert.Throws<InvalidOperationException>(delegate
            {
                eventService.SetEventKey<TestEvent>(new object());
            });
        }
    }
}
