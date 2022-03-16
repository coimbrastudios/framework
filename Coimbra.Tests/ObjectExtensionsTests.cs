using NUnit.Framework;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.TestTools;

namespace Coimbra.Tests
{
    [TestFixture]
    [TestOf(typeof(ObjectUtility))]
    public class ObjectExtensionsTests
    {
        [Test]
        public void GetValid_ReturnsValidSystemObject()
        {
            const string name = "Test";
            object o = name;
            Assert.That(o.GetValid()?.ToString() == name);
        }

        [UnityTest]
        public IEnumerator GetValid_ReturnsValidUnityObject()
        {
            const string name = "Test";
            object gameObject = new GameObject(name);

            yield return null;

            Assert.That(gameObject.GetValid()?.ToString().Contains(name) ?? false);
            UnityEngine.Object.Destroy((GameObject)gameObject);
        }

        [Test]
        [SuppressMessage("ReSharper", "ExpressionIsAlwaysNull")]
        public void GetValid_ReturnsInvalidSystemObject()
        {
            object o = null;
            Assert.That(o.GetValid()?.ToString() == null);
            Assert.Throws<NullReferenceException>(delegate
            {
                _ = o.GetValid().ToString();
            });
        }

        [UnityTest]
        public IEnumerator GetValid_ReturnsInvalidUnityObject()
        {
            const string name = "Test";
            object gameObject = new GameObject(name);

            yield return null;

            Assert.That(gameObject.GetValid()?.ToString().Contains(name) ?? false);
            UnityEngine.Object.Destroy((GameObject)gameObject);

            yield return null;

            Assert.That(gameObject.GetValid()?.ToString().Equals(name) ?? true);
            Assert.Throws<NullReferenceException>(delegate
            {
                _ = gameObject.GetValid().ToString();
            });

            Assert.DoesNotThrow(delegate
            {
                _ = gameObject.ToString();
            });
        }

        [Test]
        public void IsValid_ReturnsValidSystemObject()
        {
            const string name = "Test";
            object o = name;
            Assert.That(o.IsValid());
        }

        [UnityTest]
        public IEnumerator IsValid_ReturnsValidUnityObject()
        {
            const string name = "Test";
            object gameObject = new GameObject(name);

            yield return null;

            Assert.That(gameObject.IsValid());
            UnityEngine.Object.Destroy((GameObject)gameObject);
        }

        [Test]
        [SuppressMessage("ReSharper", "ExpressionIsAlwaysNull")]
        public void IsValid_ReturnsInvalidSystemObject()
        {
            object o = null;
            Assert.That(!o.IsValid());
        }

        [UnityTest]
        public IEnumerator IsValid_ReturnsInvalidUnityObject()
        {
            const string name = "Test";
            object gameObject = new GameObject(name);

            yield return null;

            Assert.That(gameObject.IsValid());
            UnityEngine.Object.Destroy((GameObject)gameObject);

            yield return null;

            Assert.That(gameObject.ToString() != null);
            Assert.That(!gameObject.IsValid());
        }
    }
}
