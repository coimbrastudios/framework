using NUnit.Framework;

namespace Coimbra.Tests.Editor
{
    [TestFixture]
    [TestOf(typeof(ServiceLocator))]
    internal sealed class ServiceLocatorTests
    {
        private sealed class DummyService
        {
            public int Value;
        }

        [TearDown]
        public void TearDown()
        {
            ServiceLocator.Shared.Set<DummyService>(null);
        }

        [Test]
        public void CreateCallbackWorks()
        {
            bool hasService = ServiceLocator.Shared.TryGet(out DummyService service);
            Assert.That(hasService, Is.False);
            Assert.That(service, Is.Null);

            ServiceLocator.Shared.SetCreateCallback(() => new DummyService(), true);
            Assert.That(ServiceLocator.Shared.HasCreateCallback<DummyService>(), Is.True);
            Assert.That(ServiceLocator.Shared.IsCreated<DummyService>(), Is.False);

            hasService = ServiceLocator.Shared.TryGet(out service);
            Assert.That(hasService, Is.True);
            Assert.That(service, Is.Not.Null);
            Assert.That(ServiceLocator.Shared.HasCreateCallback<DummyService>(), Is.False);

            ServiceLocator.Shared.Set<DummyService>(null);

            hasService = ServiceLocator.Shared.TryGet(out service);
            Assert.That(hasService, Is.False);
            Assert.That(service, Is.Null);
        }

        [Test]
        public void ValueChangedEventWorks([Random(1, int.MaxValue, 1)] int a, [Random(int.MinValue, 0, 1)] int b)
        {
            bool hasSingleton = ServiceLocator.Shared.TryGet(out DummyService singleton);
            Assert.That(hasSingleton, Is.False);
            Assert.That(singleton, Is.Null);

            DummyService serviceA = new DummyService
            {
                Value = a,
            };

            DummyService serviceB = new DummyService
            {
                Value = b,
            };

            static void serviceValueChangedHandler(object oldValue, object newValue)
            {
                if (oldValue is DummyService oldService && newValue is DummyService newService)
                {
                    newService.Value = oldService.Value;
                }
            }

            ServiceLocator.Shared.AddValueChangedListener<DummyService>(serviceValueChangedHandler);
            ServiceLocator.Shared.Set(serviceA);

            hasSingleton = ServiceLocator.Shared.TryGet(out singleton);
            Assert.That(hasSingleton, Is.True);
            Assert.That(singleton, Is.EqualTo(serviceA));
            Assert.That(singleton, Is.Not.Null);
            Assert.That(singleton.Value, Is.EqualTo(a));

            ServiceLocator.Shared.Set(serviceB);

            hasSingleton = ServiceLocator.Shared.TryGet(out singleton);
            Assert.That(hasSingleton, Is.True);
            Assert.That(singleton, Is.EqualTo(serviceB));
            Assert.That(singleton, Is.Not.Null);
            Assert.That(singleton.Value, Is.EqualTo(a));

            ServiceLocator.Shared.RemoveValueChangedListener<DummyService>(serviceValueChangedHandler);

            serviceB.Value = b;
            hasSingleton = ServiceLocator.Shared.TryGet(out singleton);
            Assert.That(hasSingleton, Is.True);
            Assert.That(singleton, Is.EqualTo(serviceB));
            Assert.That(singleton, Is.Not.Null);
            Assert.That(singleton.Value, Is.EqualTo(b));

            ServiceLocator.Shared.Set(serviceA);

            hasSingleton = ServiceLocator.Shared.TryGet(out singleton);
            Assert.That(hasSingleton, Is.True);
            Assert.That(singleton, Is.EqualTo(serviceA));
            Assert.That(singleton, Is.Not.Null);
            Assert.That(singleton.Value, Is.EqualTo(a));
        }
    }
}
