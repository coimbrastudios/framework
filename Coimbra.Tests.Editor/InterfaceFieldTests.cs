using NUnit.Framework;
using UnityEngine;

namespace Coimbra
{
    [TestFixture(TestOf = typeof(InterfaceField<>))]
    public sealed class InterfaceFieldTests
    {
    private struct DummyStruct : IDummyInterface
        {
            public int Number { get; set; }
        }

        private sealed class DummyClass : IDummyInterface
        {
            public int Number { get; set; }
        }

        private static IDummyInterface[] _systemObjects =
        {
            new DummyStruct(),
            new DummyClass(),
        };
        private DummyAssetWithDummyInterface _asset;

        [SetUp]
        public void SetUp()
        {
            _asset = ScriptableObject.CreateInstance<DummyAssetWithDummyInterface>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_asset);
        }

        [Test]
        public void ModifyOriginalAffectsReference_SystemObject([ValueSource(nameof(_systemObjects))] IDummyInterface dummy, [Random(1, int.MaxValue, 1)] int value)
        {
            Assert.That(_asset.InterfaceField.HasValue, Is.False);
            Assert.That(_asset.InterfaceField.IsSystemObject, Is.False);
            Assert.That(_asset.InterfaceField.IsUnityObject, Is.False);

            _asset.InterfaceField = new InterfaceField<IDummyInterface>(dummy);
            Assert.That(_asset.InterfaceField.HasValue, Is.True);
            Assert.That(_asset.InterfaceField.IsSystemObject, Is.True);
            Assert.That(_asset.InterfaceField.IsUnityObject, Is.False);

            dummy.Number = value;
            Assert.That(_asset.InterfaceField.Value.Number, Is.EqualTo(value));
            Assert.That(_asset.InterfaceField, Is.EqualTo(dummy));
        }

        [Test]
        public void ModifyOriginalAffectsReference_UnityObject([Random(1, int.MaxValue, 1)] int value)
        {
            Assert.That(_asset.InterfaceField.HasValue, Is.False);
            Assert.That(_asset.InterfaceField.IsSystemObject, Is.False);
            Assert.That(_asset.InterfaceField.IsUnityObject, Is.False);

            _asset.InterfaceField = new InterfaceField<IDummyInterface>(_asset);
            Assert.That(_asset.InterfaceField.HasValue, Is.True);
            Assert.That(_asset.InterfaceField.IsSystemObject, Is.False);
            Assert.That(_asset.InterfaceField.IsUnityObject, Is.True);

            _asset.Number = value;
            Assert.That(_asset.InterfaceField.Value.Number, Is.EqualTo(value));
            Assert.That(_asset.InterfaceField, Is.EqualTo(_asset));
        }

        [Test]
        public void ModifyReferenceAffectsOriginal_SystemObject([ValueSource(nameof(_systemObjects))] IDummyInterface dummy, [Random(1, int.MaxValue, 1)] int value)
        {
            Assert.That(_asset.InterfaceField.HasValue, Is.False);
            Assert.That(_asset.InterfaceField.IsSystemObject, Is.False);
            Assert.That(_asset.InterfaceField.IsUnityObject, Is.False);

            _asset.InterfaceField = new InterfaceField<IDummyInterface>(dummy);
            Assert.That(_asset.InterfaceField.HasValue, Is.True);
            Assert.That(_asset.InterfaceField.IsSystemObject, Is.True);
            Assert.That(_asset.InterfaceField.IsUnityObject, Is.False);

            _asset.InterfaceField.Value.Number = value;
            Assert.That(dummy.Number, Is.EqualTo(value));
            Assert.That(dummy, Is.EqualTo(_asset.InterfaceField));
        }

        [Test]
        public void ModifyReferenceAffectsOriginal_UnityObject([Random(1, int.MaxValue, 1)] int value)
        {
            Assert.That(_asset.InterfaceField.HasValue, Is.False);
            Assert.That(_asset.InterfaceField.IsSystemObject, Is.False);
            Assert.That(_asset.InterfaceField.IsUnityObject, Is.False);

            _asset.InterfaceField = new InterfaceField<IDummyInterface>(_asset);
            Assert.That(_asset.InterfaceField.HasValue, Is.True);
            Assert.That(_asset.InterfaceField.IsSystemObject, Is.False);
            Assert.That(_asset.InterfaceField.IsUnityObject, Is.True);

            _asset.InterfaceField.Value.Number = value;
            Assert.That(_asset.Number, Is.EqualTo(value));
            Assert.That(_asset, Is.EqualTo(_asset.InterfaceField));
        }
    }
}
