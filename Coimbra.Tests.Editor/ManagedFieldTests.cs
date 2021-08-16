using NUnit.Framework;
using UnityEngine;

namespace Coimbra.Tests.Editor
{
    [TestFixture]
    [TestOf(typeof(ManagedField<>))]
    internal sealed class ManagedFieldTests
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
            Assert.That(_asset.InterfaceField.HasValue, Is.False);
            Assert.That(_asset.InterfaceField.IsSystemObject, Is.False);
            Assert.That(_asset.InterfaceField.IsUnityObject, Is.False);

            _asset.InterfaceField = new ManagedField<IDummyInterface>(dummy);
            Assert.That(_asset.InterfaceField.HasValue, Is.True);
            Assert.That(_asset.InterfaceField.IsSystemObject, Is.True);
            Assert.That(_asset.InterfaceField.IsUnityObject, Is.False);

            dummy.Integer = value;
            Assert.That(_asset.InterfaceField.Value.Integer, Is.EqualTo(value));
            Assert.That(_asset.InterfaceField, Is.EqualTo(dummy));
        }

        [Test]
        public void ModifyOriginalAffectsReference_UnityObject([Random(1, int.MaxValue, 1)] int value)
        {
            Assert.That(_asset.InterfaceField.HasValue, Is.False);
            Assert.That(_asset.InterfaceField.IsSystemObject, Is.False);
            Assert.That(_asset.InterfaceField.IsUnityObject, Is.False);

            _asset.InterfaceField = new ManagedField<IDummyInterface>(_asset);
            Assert.That(_asset.InterfaceField.HasValue, Is.True);
            Assert.That(_asset.InterfaceField.IsSystemObject, Is.False);
            Assert.That(_asset.InterfaceField.IsUnityObject, Is.True);

            _asset.Integer = value;
            Assert.That(_asset.InterfaceField.Value.Integer, Is.EqualTo(value));
            Assert.That(_asset.InterfaceField, Is.EqualTo(_asset));
        }

        [Test]
        public void ModifyReferenceAffectsOriginal_SystemObject([ValueSource(nameof(_systemObjects))] IDummyInterface dummy, [Random(1, int.MaxValue, 1)] int value)
        {
            Assert.That(_asset.InterfaceField.HasValue, Is.False);
            Assert.That(_asset.InterfaceField.IsSystemObject, Is.False);
            Assert.That(_asset.InterfaceField.IsUnityObject, Is.False);

            _asset.InterfaceField = new ManagedField<IDummyInterface>(dummy);
            Assert.That(_asset.InterfaceField.HasValue, Is.True);
            Assert.That(_asset.InterfaceField.IsSystemObject, Is.True);
            Assert.That(_asset.InterfaceField.IsUnityObject, Is.False);

            _asset.InterfaceField.Value.Integer = value;
            Assert.That(dummy.Integer, Is.EqualTo(value));
            Assert.That(dummy, Is.EqualTo(_asset.InterfaceField));
        }

        [Test]
        public void ModifyReferenceAffectsOriginal_UnityObject([Random(1, int.MaxValue, 1)] int value)
        {
            Assert.That(_asset.InterfaceField.HasValue, Is.False);
            Assert.That(_asset.InterfaceField.IsSystemObject, Is.False);
            Assert.That(_asset.InterfaceField.IsUnityObject, Is.False);

            _asset.InterfaceField = new ManagedField<IDummyInterface>(_asset);
            Assert.That(_asset.InterfaceField.HasValue, Is.True);
            Assert.That(_asset.InterfaceField.IsSystemObject, Is.False);
            Assert.That(_asset.InterfaceField.IsUnityObject, Is.True);

            _asset.InterfaceField.Value.Integer = value;
            Assert.That(_asset.Integer, Is.EqualTo(value));
            Assert.That(_asset, Is.EqualTo(_asset.InterfaceField));
        }
    }
}
