using NUnit.Framework;
using UnityEngine;

namespace Coimbra.Tests
{
    [TestFixture]
    [TestOf(typeof(ManagedField<>))]
    public class ManagedFieldTests
    {
        private static IDummyInterface[] _systemObjects =
        {
            new DummyStruct(),
            new DummyClass(),
        };

        private DummyAsset _asset;

        [SetUp]
        public void SetUp()
        {
            _asset = ScriptableObject.CreateInstance<DummyAsset>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_asset);
        }

        [Test]
        public void ModifyOriginalAffectsReference_SystemObject([ValueSource(nameof(_systemObjects))] IDummyInterface dummy, [Random(1, int.MaxValue, 1)] int value)
        {
            Assert.That(_asset.ManagedField.HasValue, Is.False);
            Assert.That(_asset.ManagedField.IsSystemObject, Is.False);
            Assert.That(_asset.ManagedField.IsUnityObject, Is.False);

            _asset.ManagedField = new ManagedField<IDummyInterface>(dummy);
            Assert.That(_asset.ManagedField.HasValue, Is.True);
            Assert.That(_asset.ManagedField.IsSystemObject, Is.True);
            Assert.That(_asset.ManagedField.IsUnityObject, Is.False);

            dummy.Integer = value;
            Assert.That(_asset.ManagedField.Value, Is.Not.Null);
            Assert.That(_asset.ManagedField.Value.Integer, Is.EqualTo(value));
            Assert.That(_asset.ManagedField, Is.EqualTo(dummy));
        }

        [Test]
        public void ModifyOriginalAffectsReference_UnityObject([Random(1, int.MaxValue, 1)] int value)
        {
            Assert.That(_asset.ManagedField.HasValue, Is.False);
            Assert.That(_asset.ManagedField.IsSystemObject, Is.False);
            Assert.That(_asset.ManagedField.IsUnityObject, Is.False);

            _asset.ManagedField = new ManagedField<IDummyInterface>(_asset);
            Assert.That(_asset.ManagedField.HasValue, Is.True);
            Assert.That(_asset.ManagedField.IsSystemObject, Is.False);
            Assert.That(_asset.ManagedField.IsUnityObject, Is.True);

            _asset.Integer = value;
            Assert.That(_asset.ManagedField.Value, Is.Not.Null);
            Assert.That(_asset.ManagedField.Value.Integer, Is.EqualTo(value));
            Assert.That(_asset.ManagedField, Is.EqualTo(_asset));
        }

        [Test]
        public void ModifyReferenceAffectsOriginal_SystemObject([ValueSource(nameof(_systemObjects))] IDummyInterface dummy, [Random(1, int.MaxValue, 1)] int value)
        {
            Assert.That(_asset.ManagedField.HasValue, Is.False);
            Assert.That(_asset.ManagedField.IsSystemObject, Is.False);
            Assert.That(_asset.ManagedField.IsUnityObject, Is.False);

            _asset.ManagedField = new ManagedField<IDummyInterface>(dummy);
            Assert.That(_asset.ManagedField.HasValue, Is.True);
            Assert.That(_asset.ManagedField.IsSystemObject, Is.True);
            Assert.That(_asset.ManagedField.IsUnityObject, Is.False);
            Assert.That(_asset.ManagedField.Value, Is.Not.Null);

            _asset.ManagedField.Value.Integer = value;
            Assert.That(dummy.Integer, Is.EqualTo(value));
            Assert.That(dummy, Is.EqualTo(_asset.ManagedField));
        }

        [Test]
        public void ModifyReferenceAffectsOriginal_UnityObject([Random(1, int.MaxValue, 1)] int value)
        {
            Assert.That(_asset.ManagedField.HasValue, Is.False);
            Assert.That(_asset.ManagedField.IsSystemObject, Is.False);
            Assert.That(_asset.ManagedField.IsUnityObject, Is.False);

            _asset.ManagedField = new ManagedField<IDummyInterface>(_asset);
            Assert.That(_asset.ManagedField.HasValue, Is.True);
            Assert.That(_asset.ManagedField.IsSystemObject, Is.False);
            Assert.That(_asset.ManagedField.IsUnityObject, Is.True);
            Assert.That(_asset.ManagedField.Value, Is.Not.Null);

            _asset.ManagedField.Value.Integer = value;
            Assert.That(_asset.Integer, Is.EqualTo(value));
            Assert.That(_asset, Is.EqualTo(_asset.ManagedField));
        }
    }
}
