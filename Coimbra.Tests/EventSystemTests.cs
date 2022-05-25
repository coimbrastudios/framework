using Coimbra.Services.Events;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.TestTools;

namespace Coimbra.Tests
{
    public readonly partial struct TestEvent : IEvent { }

    [TestFixture]
    [TestOf(typeof(EventSystem))]
    public class EventSystemTests
    {
        private IEventService _eventService;

        [SetUp]
        public void SetUp()
        {
            _eventService = new EventSystem();
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

            TestEvent.AddListenerAt(_eventService, delegate
            {
                Debug.Log(log);
            });

            LogAssert.Expect(LogType.Log, log);
            new TestEvent().InvokeAt(_eventService, this);
        }

        [Test]
        public void AddListener_Multiple()
        {
            const string logA = nameof(logA);
            const string logB = nameof(logB);

            TestEvent.AddListenerAt(_eventService, delegate
            {
                Debug.Log(logA);
            });

            TestEvent.AddListenerAt(_eventService, delegate
            {
                Debug.Log(logB);
            });

            LogAssert.Expect(LogType.Log, logA);
            LogAssert.Expect(LogType.Log, logB);
            new TestEvent().InvokeAt(_eventService, this);
        }

        [Test]
        [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
        public void RemoveListener_WhenInvoking()
        {
            EventHandle handle1 = new EventHandle();
            EventHandle handle2 = new EventHandle();
            EventHandle handle3 = new EventHandle();
            EventHandle handle4 = new EventHandle();

            void callback1(ref Event<TestEvent> testEvent)
            {
                Debug.Log(nameof(callback1));
                _eventService.RemoveListener(handle1);
            }

            void callback2(ref Event<TestEvent> testEvent)
            {
                Debug.Log(nameof(callback2));
                _eventService.RemoveListener(handle2);
            }

            void callback3(ref Event<TestEvent> testEvent)
            {
                Debug.Log(nameof(callback3));
                _eventService.RemoveListener(handle3);
            }

            void callback4(ref Event<TestEvent> testEvent)
            {
                Debug.Log(nameof(callback4));
                _eventService.RemoveListener(handle4);
            }

            handle1 = TestEvent.AddListenerAt(_eventService, callback1);
            handle2 = TestEvent.AddListenerAt(_eventService, callback2);
            handle3 = TestEvent.AddListenerAt(_eventService, callback3);
            handle4 = TestEvent.AddListenerAt(_eventService, callback4);

            LogAssert.Expect(LogType.Log, nameof(callback1));
            LogAssert.Expect(LogType.Log, nameof(callback2));
            LogAssert.Expect(LogType.Log, nameof(callback3));
            LogAssert.Expect(LogType.Log, nameof(callback4));
            new TestEvent().InvokeAt(_eventService, this);
            LogAssert.NoUnexpectedReceived();
            new TestEvent().InvokeAt(_eventService, this);
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void RemoveListener_Empty()
        {
            _eventService.RemoveListener(EventHandle.Create(typeof(TestEvent)));
            new TestEvent().InvokeAt(_eventService, this);
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void RemoveListener_Single()
        {
            const string log = nameof(log);

            static void callback(ref Event<TestEvent> testEvent)
            {
                Debug.Log(log);
            }

            EventHandle handle = TestEvent.AddListenerAt(_eventService, callback);
            _eventService.RemoveListener(handle);
            new TestEvent().InvokeAt(_eventService, this);
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void RemoveListener_Multiple()
        {
            const string logA = nameof(logA);
            const string logB = nameof(logB);

            static void callbackA(ref Event<TestEvent> testEvent)
            {
                Debug.Log(logA);
            }

            static void callbackB(ref Event<TestEvent> testEvent)
            {
                Debug.Log(logB);
            }

            EventHandle handle = TestEvent.AddListenerAt(_eventService, callbackA);
            TestEvent.AddListenerAt(_eventService, callbackB);
            _eventService.RemoveListener(handle);

            LogAssert.Expect(LogType.Log, logB);
            new TestEvent().InvokeAt(_eventService, this);
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void RemoveAllListeners_Empty()
        {
            TestEvent.RemoveAllListenersAt(_eventService);
            new TestEvent().InvokeAt(_eventService, this);
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void RemoveAllListeners_Multiple()
        {
            const string logA = nameof(logA);
            const string logB = nameof(logB);

            TestEvent.AddListenerAt(_eventService, delegate
            {
                Debug.Log(logA);
            });

            TestEvent.AddListenerAt(_eventService, delegate
            {
                Debug.Log(logB);
            });

            TestEvent.RemoveAllListenersAt(_eventService);
            new TestEvent().InvokeAt(_eventService, this);
            LogAssert.NoUnexpectedReceived();
        }
    }
}
