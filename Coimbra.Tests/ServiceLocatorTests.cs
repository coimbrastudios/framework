using Coimbra.Services;
using NUnit.Framework;

namespace Coimbra.Tests
{
    [TestFixture]
    [TestOf(typeof(ServiceLocator))]
    public class ServiceLocatorTests
    {
        private sealed class DummyServiceFactory : IServiceFactory
        {
            public static readonly DummyServiceFactory Instance = new DummyServiceFactory();

            private DummyServiceFactory() { }

            public IService Create(ServiceLocator owningLocator)
            {
                return new DummyService();
            }
        }

        private interface IDummyService : IService
        {
            int Value { get; set; }
        }

        [DisableDefaultFactory]
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

            ServiceLocator.Shared.SetFactory<IDummyService>(DummyServiceFactory.Instance);
            Assert.That(ServiceLocator.Shared.HasFactory<IDummyService>(), Is.True);
            Assert.That(ServiceLocator.Shared.IsCreated<IDummyService>(), Is.False);

            hasService = ServiceLocator.Shared.TryGet(out service);
            Assert.That(hasService, Is.True);
            Assert.That(service, Is.Not.Null);
        }
    }
}
