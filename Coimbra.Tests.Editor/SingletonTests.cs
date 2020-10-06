using NUnit.Framework;

namespace Coimbra.Tests.Editor
{
    [TestFixture]
    [TestOf(typeof(Singleton<>))]
    internal sealed class SingletonTests
    {
        private sealed class DummySingleton
        {
            public int Value;
        }

        [TearDown]
        public void TearDown()
        {
            Singleton<DummySingleton>.Set(null);
        }

        [Test]
        public void CreateCallbackWorks()
        {
            bool hasSingleton = Singleton<DummySingleton>.TryGet(out DummySingleton singleton);
            Assert.That(hasSingleton, Is.False);
            Assert.That(singleton, Is.Null);

            singleton = Singleton<DummySingleton>.GetOrCreate();
            Assert.That(singleton, Is.Null);

            Singleton<DummySingleton>.SetCreateCallback(() => new DummySingleton());

            hasSingleton = Singleton<DummySingleton>.TryGet(out singleton);
            Assert.That(hasSingleton, Is.False);
            Assert.That(singleton, Is.Null);

            singleton = Singleton<DummySingleton>.GetOrCreate();
            Assert.That(singleton, Is.Not.Null);

            hasSingleton = Singleton<DummySingleton>.TryGet(out singleton);
            Assert.That(hasSingleton, Is.True);
            Assert.That(singleton, Is.Not.Null);

            Singleton<DummySingleton>.Set(null);

            hasSingleton = Singleton<DummySingleton>.TryGet(out singleton);
            Assert.That(hasSingleton, Is.False);
            Assert.That(singleton, Is.Null);

            Singleton<DummySingleton>.SetCreateCallback(null);

            singleton = Singleton<DummySingleton>.GetOrCreate();
            Assert.That(singleton, Is.Null);
        }

        [Test]
        public void ValueChangedEventWorks([Random(1, int.MaxValue, 1)] int a, [Random(int.MinValue, 0, 1)] int b)
        {
            bool hasSingleton = Singleton<DummySingleton>.TryGet(out DummySingleton singleton);
            Assert.That(hasSingleton, Is.False);
            Assert.That(singleton, Is.Null);

            DummySingleton singletonA = new DummySingleton
            {
                Value = a,
            };

            DummySingleton singletonB = new DummySingleton
            {
                Value = b,
            };

            void singletonChangedHandler(DummySingleton oldValue, DummySingleton newValue)
            {
                if (oldValue != null && newValue != null)
                {
                    newValue.Value = oldValue.Value;
                }
            }

            Singleton<DummySingleton>.OnValueChanged += singletonChangedHandler;
            Singleton<DummySingleton>.Set(singletonA);

            hasSingleton = Singleton<DummySingleton>.TryGet(out singleton);
            Assert.That(hasSingleton, Is.True);
            Assert.That(singleton, Is.EqualTo(singletonA));
            Assert.That(singleton, Is.Not.Null);
            Assert.That(singleton.Value, Is.EqualTo(a));

            Singleton<DummySingleton>.Set(singletonB);

            hasSingleton = Singleton<DummySingleton>.TryGet(out singleton);
            Assert.That(hasSingleton, Is.True);
            Assert.That(singleton, Is.EqualTo(singletonB));
            Assert.That(singleton, Is.Not.Null);
            Assert.That(singleton.Value, Is.EqualTo(a));

            Singleton<DummySingleton>.OnValueChanged -= singletonChangedHandler;

            singletonB.Value = b;
            hasSingleton = Singleton<DummySingleton>.TryGet(out singleton);
            Assert.That(hasSingleton, Is.True);
            Assert.That(singleton, Is.EqualTo(singletonB));
            Assert.That(singleton, Is.Not.Null);
            Assert.That(singleton.Value, Is.EqualTo(b));

            Singleton<DummySingleton>.Set(singletonA);

            hasSingleton = Singleton<DummySingleton>.TryGet(out singleton);
            Assert.That(hasSingleton, Is.True);
            Assert.That(singleton, Is.EqualTo(singletonA));
            Assert.That(singleton, Is.Not.Null);
            Assert.That(singleton.Value, Is.EqualTo(a));
        }
    }
}
