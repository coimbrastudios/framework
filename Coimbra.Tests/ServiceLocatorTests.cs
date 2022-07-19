using Coimbra.Services;
using NUnit.Framework;

namespace Coimbra.Tests
{
    [TestFixture]
    [TestOf(typeof(ServiceLocator))]
    public class ServiceLocatorTests
    {
        [DynamicService]
        [HideInServiceLocatorWindow]
        private interface IDummyService : IService
        {
            int Value { get; set; }
        }

        private sealed class DummyServiceFactory : IServiceFactory
        {
            public static readonly DummyServiceFactory Instance = new();

            private DummyServiceFactory() { }

            public IService GetService()
            {
                return new DummyService();
            }
        }

        [DisableDefaultFactory]
        [HideInServiceLocatorWindow]
        private sealed class DummyService : IDummyService
        {
            public int Value { get; set; }

            public void Dispose() { }
        }

        [Test]
        public void CreateCallbackWorks()
        {
            bool hasService = ServiceLocator.TryGet(out IDummyService service);
            Assert.That(hasService, Is.False);
            Assert.That(service, Is.Null);

            ServiceLocator.SetFactory<IDummyService>(DummyServiceFactory.Instance);
            Assert.That(ServiceLocator.HasFactory<IDummyService>(), Is.True);
            Assert.That(ServiceLocator.IsSet<IDummyService>(), Is.False);

            hasService = ServiceLocator.TryGet(out service);
            Assert.That(hasService, Is.True);
            Assert.That(service, Is.Not.Null);
            ServiceLocator.Set<IDummyService>(null);
        }
    }
}
