using Coimbra.Services.Events;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.TestTools;

namespace Coimbra.Tests
{
#pragma warning disable SA1402
    public readonly partial struct TestEvent : IEvent { }
#pragma warning restore SA1402

    [TestFixture]
    [TestOf(typeof(EventSystem))]
    public class EventSystemTests
    {
        private EventSystem _eventSystem;

        [SetUp]
        public void SetUp()
        {
            _eventSystem = new EventSystem();
        }

        [TearDown]
        public void TearDown()
        {
            _eventSystem.Dispose();
            _eventSystem = null;
        }

        [Test]
        public void AddListener_Single()
        {
            const string log = nameof(log);

            _eventSystem.AddListener<TestEvent>(delegate
            {
                Debug.Log(log);
            });

            LogAssert.Expect(LogType.Log, log);
            _eventSystem.Invoke(this, default(TestEvent));
        }

        [Test]
        public void AddListener_Multiple()
        {
            const string logA = nameof(logA);
            const string logB = nameof(logB);

            _eventSystem.AddListener<TestEvent>(delegate
            {
                Debug.Log(logA);
            });

            _eventSystem.AddListener<TestEvent>(delegate
            {
                Debug.Log(logB);
            });

            LogAssert.Expect(LogType.Log, logA);
            LogAssert.Expect(LogType.Log, logB);
            _eventSystem.Invoke(this, default(TestEvent));
        }

        [Test]
        [SuppressMessage("ReSharper", "AccessToModifiedClosure", Justification = "Values are temporary values for testing.")]
        public void RemoveListener_WhenInvoking()
        {
            EventHandle handle1 = default;
            EventHandle handle2 = default;
            EventHandle handle3 = default;
            EventHandle handle4 = default;

            void callback1(ref EventContext context, in TestEvent e)
            {
                Debug.Log(nameof(callback1));
                _eventSystem.RemoveListener(handle1);
            }

            void callback2(ref EventContext context, in TestEvent e)
            {
                Debug.Log(nameof(callback2));
                _eventSystem.RemoveListener(handle2);
            }

            void callback3(ref EventContext context, in TestEvent e)
            {
                Debug.Log(nameof(callback3));
                _eventSystem.RemoveListener(handle3);
            }

            void callback4(ref EventContext context, in TestEvent e)
            {
                Debug.Log(nameof(callback4));
                _eventSystem.RemoveListener(handle4);
            }

            handle1 = _eventSystem.AddListener<TestEvent>(callback1);
            handle2 = _eventSystem.AddListener<TestEvent>(callback2);
            handle3 = _eventSystem.AddListener<TestEvent>(callback3);
            handle4 = _eventSystem.AddListener<TestEvent>(callback4);

            LogAssert.Expect(LogType.Log, nameof(callback1));
            LogAssert.Expect(LogType.Log, nameof(callback2));
            LogAssert.Expect(LogType.Log, nameof(callback3));
            LogAssert.Expect(LogType.Log, nameof(callback4));
            _eventSystem.Invoke(this, default(TestEvent));
            LogAssert.NoUnexpectedReceived();
            _eventSystem.Invoke(this, default(TestEvent));
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void RemoveListener_Empty()
        {
            _eventSystem.RemoveListener(EventHandle.Create(_eventSystem, typeof(TestEvent)));
            _eventSystem.Invoke(this, default(TestEvent));
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void RemoveListener_Single()
        {
            const string log = nameof(log);

            static void callback(ref EventContext context, in TestEvent e)
            {
                Debug.Log(log);
            }

            EventHandle handle = _eventSystem.AddListener<TestEvent>(callback);
            _eventSystem.RemoveListener(handle);
            _eventSystem.Invoke(this, default(TestEvent));
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void RemoveListener_Multiple()
        {
            const string logA = nameof(logA);
            const string logB = nameof(logB);

            static void callbackA(ref EventContext context, in TestEvent e)
            {
                Debug.Log(logA);
            }

            static void callbackB(ref EventContext context, in TestEvent e)
            {
                Debug.Log(logB);
            }

            EventHandle handle = _eventSystem.AddListener<TestEvent>(callbackA);
            _eventSystem.AddListener<TestEvent>(callbackB);
            _eventSystem.RemoveListener(handle);

            LogAssert.Expect(LogType.Log, logB);
            _eventSystem.Invoke(this, default(TestEvent));
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void RemoveAllListeners_Empty()
        {
            _eventSystem.RemoveAllListeners<TestEvent>();
            _eventSystem.Invoke(this, default(TestEvent));
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void RemoveAllListeners_Multiple()
        {
            const string logA = nameof(logA);
            const string logB = nameof(logB);

            _eventSystem.AddListener<TestEvent>(delegate
            {
                Debug.Log(logA);
            });

            _eventSystem.AddListener<TestEvent>(delegate
            {
                Debug.Log(logB);
            });

            _eventSystem.RemoveAllListeners<TestEvent>();
            _eventSystem.Invoke(this, default(TestEvent));
            LogAssert.NoUnexpectedReceived();
        }
    }
}
