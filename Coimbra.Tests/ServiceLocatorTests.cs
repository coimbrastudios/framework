using NUnit.Framework;

namespace Coimbra.Tests
{
    [TestFixture]
    [TestOf(typeof(ServiceLocator))]
    public class ServiceLocatorTests
    {
        private interface IDummyService : IService
        {
            int Value { get; set; }
        }

        private sealed class DummyService : IDummyService
        {
            public int Value { get; set; }

            public void Dispose() { }

            public ServiceLocator OwningLocator { get; set; }
        }

        [TearDown]
        public void TearDown()
        {
            ServiceLocator.Shared.Dispose();
        }

        [Test]
        public void CreateCallbackWorks()
        {
            bool hasService = ServiceLocator.Shared.TryGet(out IDummyService service);
            Assert.That(hasService, Is.False);
            Assert.That(service, Is.Null);

            ServiceLocator.Shared.SetCreateCallback<IDummyService>(() => new DummyService(), true);
            Assert.That(ServiceLocator.Shared.HasCreateCallback<IDummyService>(), Is.True);
            Assert.That(ServiceLocator.Shared.IsCreated<IDummyService>(), Is.False);

            hasService = ServiceLocator.Shared.TryGet(out service);
            Assert.That(hasService, Is.True);
            Assert.That(service, Is.Not.Null);
            Assert.That(ServiceLocator.Shared.HasCreateCallback<IDummyService>(), Is.False);

            ServiceLocator.Shared.Set<IDummyService>(null);

            hasService = ServiceLocator.Shared.TryGet(out service);
            Assert.That(hasService, Is.False);
            Assert.That(service, Is.Null);
        }

        [Test]
        public void ValueChangedEventWorks([Random(1, int.MaxValue, 1)] int a, [Random(int.MinValue, 0, 1)] int b)
        {
            bool hasService = ServiceLocator.Shared.TryGet(out IDummyService service);
            Assert.That(hasService, Is.False);
            Assert.That(service, Is.Null);

            IDummyService serviceA = new DummyService
            {
                Value = a,
            };

            IDummyService serviceB = new DummyService
            {
                Value = b,
            };

            static void serviceValueChangedHandler(object oldValue, object newValue)
            {
                if (oldValue is IDummyService oldService && newValue is IDummyService newService)
                {
                    newService.Value = oldService.Value;
                }
            }

            ServiceLocator.Shared.AddValueChangedListener<IDummyService>(serviceValueChangedHandler);
            ServiceLocator.Shared.Set(serviceA);

            hasService = ServiceLocator.Shared.TryGet(out service);
            Assert.That(hasService, Is.True);
            Assert.That(service, Is.EqualTo(serviceA));
            Assert.That(service, Is.Not.Null);
            Assert.That(service.Value, Is.EqualTo(a));

            ServiceLocator.Shared.Set(serviceB);

            hasService = ServiceLocator.Shared.TryGet(out service);
            Assert.That(hasService, Is.True);
            Assert.That(service, Is.EqualTo(serviceB));
            Assert.That(service, Is.Not.Null);
            Assert.That(service.Value, Is.EqualTo(a));

            ServiceLocator.Shared.RemoveValueChangedListener<IDummyService>(serviceValueChangedHandler);

            serviceB.Value = b;
            hasService = ServiceLocator.Shared.TryGet(out service);
            Assert.That(hasService, Is.True);
            Assert.That(service, Is.EqualTo(serviceB));
            Assert.That(service, Is.Not.Null);
            Assert.That(service.Value, Is.EqualTo(b));

            ServiceLocator.Shared.Set(serviceA);

            hasService = ServiceLocator.Shared.TryGet(out service);
            Assert.That(hasService, Is.True);
            Assert.That(service, Is.EqualTo(serviceA));
            Assert.That(service, Is.Not.Null);
            Assert.That(service.Value, Is.EqualTo(a));
        }
    }
}
