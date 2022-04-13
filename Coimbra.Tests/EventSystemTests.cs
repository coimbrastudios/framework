using Coimbra.Services;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.TestTools;

namespace Coimbra.Tests
{
    [TestFixture]
    [TestOf(typeof(EventSystem))]
    public class EventSystemTests
    {
        private struct TestEvent : IEvent { }

        private IEventService _eventService;

        [SetUp]
        public void SetUp()
        {
            _eventService = EventSystem.Create();
            Assert.IsNotNull(_eventService);
        }

        [TearDown]
        public void TearDown()
        {
            _eventService.Dispose();
            _eventService = null;
        }

        [Test]
        public void AddListener_Single()
        {
            const string log = nameof(log);
            _eventService.AddListener(delegate(ref EventData<TestEvent> testEvent)
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
            _eventService.AddListener(delegate(ref EventData<TestEvent> testEvent)
            {
                Debug.Log(logA);
            });

            _eventService.AddListener(delegate(ref EventData<TestEvent> testEvent)
            {
                Debug.Log(logB);
            });

            LogAssert.Expect(LogType.Log, logA);
            LogAssert.Expect(LogType.Log, logB);
            _eventService.Invoke(this, new TestEvent());
        }

        [Test]
        [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
        public void RemoveListener_WhenInvoking()
        {
            EventHandle handle1 = new();
            EventHandle handle2 = new();
            EventHandle handle3 = new();
            EventHandle handle4 = new();

            void callback1(ref EventData<TestEvent> testEvent)
            {
                Debug.Log(nameof(callback1));
                _eventService.RemoveListener(handle1);
            }

            void callback2(ref EventData<TestEvent> testEvent)
            {
                Debug.Log(nameof(callback2));
                _eventService.RemoveListener(handle2);
            }

            void callback3(ref EventData<TestEvent> testEvent)
            {
                Debug.Log(nameof(callback3));
                _eventService.RemoveListener(handle3);
            }

            void callback4(ref EventData<TestEvent> testEvent)
            {
                Debug.Log(nameof(callback4));
                _eventService.RemoveListener(handle4);
            }

            handle1 = _eventService.AddListener<TestEvent>(callback1);
            handle2 = _eventService.AddListener<TestEvent>(callback2);
            handle3 = _eventService.AddListener<TestEvent>(callback3);
            handle4 = _eventService.AddListener<TestEvent>(callback4);

            LogAssert.Expect(LogType.Log, nameof(callback1));
            LogAssert.Expect(LogType.Log, nameof(callback2));
            LogAssert.Expect(LogType.Log, nameof(callback3));
            LogAssert.Expect(LogType.Log, nameof(callback4));
            _eventService.Invoke(this, new TestEvent());
            LogAssert.NoUnexpectedReceived();
            _eventService.Invoke(this, new TestEvent());
            LogAssert.NoUnexpectedReceived();
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

            static void callback(ref EventData<TestEvent> testEvent)
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

            static void callbackA(ref EventData<TestEvent> testEvent)
            {
                Debug.Log(logA);
            }

            static void callbackB(ref EventData<TestEvent> testEvent)
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
            _eventService.AddListener(delegate(ref EventData<TestEvent> testEvent)
            {
                Debug.Log(logA);
            });

            _eventService.AddListener(delegate(ref EventData<TestEvent> testEvent)
            {
                Debug.Log(logB);
            });

            _eventService.RemoveAllListeners<TestEvent>();
            _eventService.Invoke(this, new TestEvent());
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void Invoke_LogsError_AfterSetEventKey()
        {
            const string log = nameof(log);
            _eventService.SetEventKey<TestEvent>(new EventKey());
            _eventService.AddListener(delegate(ref EventData<TestEvent> testEvent)
            {
                Debug.Log(log);
            });

            LogAssert.Expect(LogType.Error, new Regex(".*"));
            _eventService.Invoke(this, new TestEvent());
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void Invoke_AfterSetEventKey_WithCorrectKey()
        {
            const string log = nameof(log);
            EventKey eventKey = new();
            _eventService.SetEventKey<TestEvent>(eventKey);
            _eventService.AddListener(delegate(ref EventData<TestEvent> testEvent)
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
            _eventService.AddListener(delegate(ref EventData<TestEvent> testEvent)
            {
                Debug.Log(log);
            });

            LogAssert.Expect(LogType.Log, log);
            EventKey eventKey = new();
            _eventService.SetEventKey<TestEvent>(eventKey);
            _eventService.ResetEventKey<TestEvent>(eventKey);
            Assert.DoesNotThrow(delegate
            {
                _eventService.Invoke(this, new TestEvent());
            });
        }

        [Test]
        public void ResetEventKey_LogsError_WithWrongKey()
        {
            Assert.DoesNotThrow(delegate
            {
                _eventService.SetEventKey<TestEvent>(new EventKey());
            });

            LogAssert.Expect(LogType.Error, new Regex(".*"));
            _eventService.ResetEventKey<TestEvent>(new EventKey());
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void SetEventKey_LogsError_AfterSetEventKey()
        {
            Assert.DoesNotThrow(delegate
            {
                _eventService.SetEventKey<TestEvent>(new EventKey());
            });

            LogAssert.Expect(LogType.Error, new Regex(".*"));
            _eventService.SetEventKey<TestEvent>(new EventKey());
            LogAssert.NoUnexpectedReceived();
        }
    }
}
