using Coimbra.Tests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor.Tests
{
    [TestFixture]
    [TestOf(typeof(PropertyPathInfo))]
    internal class PropertyPathInfoTests
    {
        private const int ArrayLenght = 10;
        private DummyAsset[] _assetArray;
        private DummyBehaviour[] _behaviourArray;

        [SetUp]
        public void SetUp()
        {
            int[] integerArray =
            {
                10,
                30,
                41023,
                65904709,
                -1230213,
            };

            string[] stringArray =
            {
                "faa",
                "foo",
                "bar",
            };

            Vector3Int[] vectorArray =
            {
                Vector3Int.down,
                Vector3Int.left,
                Vector3Int.right,
                Vector3Int.one,
            };

            Texture[] textureArray =
            {
                Texture2D.blackTexture,
                Texture2D.whiteTexture,
            };

            _assetArray = new DummyAsset[ArrayLenght];
            _behaviourArray = new DummyBehaviour[ArrayLenght];

            for (int i = 0; i < ArrayLenght; i++)
            {
                _assetArray[i] = ScriptableObject.CreateInstance<DummyAsset>();
                _assetArray[i].name = $"Dummy Asset {i}";
                _assetArray[i].Initialize(_assetArray, _behaviourArray, integerArray, stringArray, textureArray, vectorArray);

                _behaviourArray[i] = new GameObject($"Dummy Object {i}").AddComponent<DummyBehaviour>();
                _behaviourArray[i].Initialize(_assetArray, _behaviourArray, integerArray, stringArray, textureArray, vectorArray);
            }
        }

        [TearDown]
        public void TearDown()
        {
            foreach (DummyAsset asset in _assetArray)
            {
                if (asset != null)
                {
                    UnityEngine.Object.DestroyImmediate(asset);
                }
            }

            foreach (DummyBehaviour behaviour in _behaviourArray)
            {
                UnityEngine.Object.DestroyImmediate(behaviour.gameObject);
            }
        }

        [Test]
        public void GetScopeWorks()
        {
            GetScopeWorks(_assetArray);
            GetScopeWorks(_behaviourArray);

            using SerializedObject serializedObject = new SerializedObject(_assetArray[0]);
            using SerializedProperty managedFieldProperty = serializedObject.FindProperty("_interfaceField");
            using SerializedProperty unityObjectProperty = managedFieldProperty.FindPropertyRelative("_unityObject");
            Assert.That(unityObjectProperty.GetScope(), Is.EqualTo(managedFieldProperty.GetValue()));
            Assert.That(managedFieldProperty.GetScope(), Is.EqualTo(_assetArray[0]));
        }

        [Test]
        public void GetValueWorks()
        {
            GetValueWorks(_assetArray);
            GetValueWorks(_behaviourArray);
        }

        [Test]
        public void SetValueWorks()
        {
            SetValueWorks(_assetArray);
            SetValueWorks(_behaviourArray);
        }

        private static void GetScopeWorks<T>(T[] targets)
            where T : UnityEngine.Object, IDummyInterface
        {
            using SerializedObject serializedObject = new SerializedObject(targets.ToArray<UnityEngine.Object>());

            Assert.That(serializedObject, Is.Not.Null);
            GetScopeWorks(serializedObject, "_integer", targets);
            GetScopeWorks(serializedObject, "_integerArray", targets);
            GetScopeWorks(serializedObject, "_string", targets);
            GetScopeWorks(serializedObject, "_stringArray", targets);
            GetScopeWorks(serializedObject, "_vector", targets);
            GetScopeWorks(serializedObject, "_vectorArray", targets);
            GetScopeWorks(serializedObject, "_vector.x", targets.Select(x => x.Vector).ToArray());
            GetScopeWorks(serializedObject, "_vector.y", targets.Select(x => x.Vector).ToArray());
            GetScopeWorks(serializedObject, "_vector.z", targets.Select(x => x.Vector).ToArray());
            GetScopeWorks(serializedObject, "_vector", "x", targets.Select(x => x.Vector).ToArray());
            GetScopeWorks(serializedObject, "_vector", "y", targets.Select(x => x.Vector).ToArray());
            GetScopeWorks(serializedObject, "_vector", "z", targets.Select(x => x.Vector).ToArray());
            GetScopeWorks(serializedObject, "_vectorArray", "x", targets.Select(x => x.VectorArray.Select(y => y).ToArray()).ToArray());
            GetScopeWorks(serializedObject, "_vectorArray", "y", targets.Select(x => x.VectorArray.Select(y => y).ToArray()).ToArray());
            GetScopeWorks(serializedObject, "_vectorArray", "z", targets.Select(x => x.VectorArray.Select(y => y).ToArray()).ToArray());
            GetScopeWorks(serializedObject, "_asset", targets);
            GetScopeWorks(serializedObject, "_assetArray", targets);
            GetScopeWorks(serializedObject, "_behaviour", targets);
            GetScopeWorks(serializedObject, "_behaviourArray", targets);
            GetScopeWorks(serializedObject, "_texture", targets);
            GetScopeWorks(serializedObject, "_textureArray", targets);
        }

        private static void GetScopeWorks<T>(SerializedObject serializedObject, string property, IReadOnlyList<T> comparers)
        {
            using SerializedProperty serializedProperty = serializedObject.FindProperty(property);

            Assert.That(serializedProperty, Is.Not.Null);
            GetScopeWorks(serializedObject, serializedProperty, comparers);
        }

        private static void GetScopeWorks<T>(SerializedObject serializedObject, string property, string relativeProperty, IReadOnlyList<T> comparers)
        {
            using SerializedProperty serializedProperty = serializedObject.FindProperty(property);

            Assert.That(serializedProperty, Is.Not.Null);

            using SerializedProperty relativeSerializedProperty = serializedProperty.FindPropertyRelative(relativeProperty);

            Assert.That(relativeSerializedProperty, Is.Not.Null);
            GetScopeWorks(serializedObject, relativeSerializedProperty, comparers);
        }

        private static void GetScopeWorks<T>(SerializedObject serializedObject, string property, string relativeProperty, IReadOnlyList<T[]> comparers)
        {
            using SerializedProperty serializedProperty = serializedObject.FindProperty(property);

            Assert.That(serializedProperty, Is.Not.Null);
            Assert.That(serializedProperty.isArray, Is.True);

            for (int i = 0; i < serializedProperty.arraySize; i++)
            {
                using SerializedProperty elementSerializedProperty = serializedProperty.GetArrayElementAtIndex(i);

                Assert.That(elementSerializedProperty, Is.Not.Null);

                using SerializedProperty relativeSerializedProperty = elementSerializedProperty.FindPropertyRelative(relativeProperty);

                Assert.That(relativeSerializedProperty, Is.Not.Null);
                GetScopeWorks(serializedObject, relativeSerializedProperty, comparers.Select(x => x[i]).ToArray());
            }
        }

        private static void GetScopeWorks<T>(SerializedObject serializedObject, SerializedProperty serializedProperty, IReadOnlyList<T> comparers)
        {
            UnityEngine.Object[] targets = serializedObject.targetObjects;
            object[] scopeArray = serializedProperty.GetScopes();
            T[] scopeArrayT = serializedProperty.GetScopes<T>();
            List<object> scopeList = new List<object>();
            List<T> scopeListT = new List<T>();
            serializedProperty.GetScopes(scopeList);
            serializedProperty.GetScopes(scopeListT);

            for (int i = 0; i < targets.Length; i++)
            {
                Assert.That(scopeArray[i], Is.EqualTo(comparers[i]));
                Assert.That(scopeArrayT[i], Is.EqualTo(comparers[i]));
                Assert.That(scopeList[i], Is.EqualTo(comparers[i]));
                Assert.That(scopeListT[i], Is.EqualTo(comparers[i]));
            }
        }

        private static void GetValueWorks<T>(T[] targets)
            where T : UnityEngine.Object, IDummyInterface
        {
            using SerializedObject serializedObject = new SerializedObject(targets.ToArray<UnityEngine.Object>());

            Assert.That(serializedObject, Is.Not.Null);
            GetValueWorks(serializedObject, "_integer", targets.Select(x => x.Integer).ToArray());
            GetValueWorks(serializedObject, "_integerArray", targets.Select(x => x.IntegerArray).ToArray());
            GetValueWorks(serializedObject, "_string", targets.Select(x => x.String).ToArray());
            GetValueWorks(serializedObject, "_stringArray", targets.Select(x => x.StringArray).ToArray());
            GetValueWorks(serializedObject, "_vector", targets.Select(x => x.Vector).ToArray());
            GetValueWorks(serializedObject, "_vectorArray", targets.Select(x => x.VectorArray).ToArray());
            GetValueWorks(serializedObject, "_vector.x", targets.Select(x => x.Vector.x).ToArray());
            GetValueWorks(serializedObject, "_vector.y", targets.Select(x => x.Vector.y).ToArray());
            GetValueWorks(serializedObject, "_vector.z", targets.Select(x => x.Vector.z).ToArray());
            GetValueWorks(serializedObject, "_vector", "x", targets.Select(x => x.Vector.x).ToArray());
            GetValueWorks(serializedObject, "_vector", "y", targets.Select(x => x.Vector.y).ToArray());
            GetValueWorks(serializedObject, "_vector", "z", targets.Select(x => x.Vector.z).ToArray());
            GetValueWorks(serializedObject, "_vectorArray", "x", targets.Select(x => x.VectorArray.Select(y => y.x).ToArray()).ToArray());
            GetValueWorks(serializedObject, "_vectorArray", "y", targets.Select(x => x.VectorArray.Select(y => y.y).ToArray()).ToArray());
            GetValueWorks(serializedObject, "_vectorArray", "z", targets.Select(x => x.VectorArray.Select(y => y.z).ToArray()).ToArray());
            GetValueWorks(serializedObject, "_asset", targets.Select(x => x.Asset).ToArray());
            GetValueWorks(serializedObject, "_assetArray", targets.Select(x => x.AssetArray).ToArray());
            GetValueWorks(serializedObject, "_behaviour", targets.Select(x => x.Behaviour).ToArray());
            GetValueWorks(serializedObject, "_behaviourArray", targets.Select(x => x.BehaviourArray).ToArray());
            GetValueWorks(serializedObject, "_texture", targets.Select(x => x.Texture).ToArray());
            GetValueWorks(serializedObject, "_textureArray", targets.Select(x => x.TextureArray).ToArray());
        }

        private static void GetValueWorks<T>(SerializedObject serializedObject, string property, IReadOnlyList<T> comparers)
        {
            using SerializedProperty serializedProperty = serializedObject.FindProperty(property);

            Assert.That(serializedProperty, Is.Not.Null);
            GetValueWorks(serializedObject, serializedProperty, comparers);
        }

        private static void GetValueWorks<T>(SerializedObject serializedObject, string property, IReadOnlyList<T[]> comparers)
        {
            using SerializedProperty serializedProperty = serializedObject.FindProperty(property);

            Assert.That(serializedProperty, Is.Not.Null);
            Assert.That(serializedProperty.isArray, Is.True);

            for (int i = 0; i < serializedProperty.arraySize; i++)
            {
                using SerializedProperty elementSerializedProperty = serializedProperty.GetArrayElementAtIndex(i);

                Assert.That(elementSerializedProperty, Is.Not.Null);
                GetValueWorks(serializedObject, elementSerializedProperty, comparers.Select(x => x[i]).ToArray());
            }
        }

        private static void GetValueWorks<T>(SerializedObject serializedObject, string property, string relativeProperty, IReadOnlyList<T> comparers)
        {
            using SerializedProperty serializedProperty = serializedObject.FindProperty(property);

            Assert.That(serializedProperty, Is.Not.Null);

            using SerializedProperty relativeSerializedProperty = serializedProperty.FindPropertyRelative(relativeProperty);

            Assert.That(relativeSerializedProperty, Is.Not.Null);
            GetValueWorks(serializedObject, relativeSerializedProperty, comparers);
        }

        private static void GetValueWorks<T>(SerializedObject serializedObject, string property, string relativeProperty, IReadOnlyList<T[]> comparers)
        {
            using SerializedProperty serializedProperty = serializedObject.FindProperty(property);

            Assert.That(serializedProperty, Is.Not.Null);
            Assert.That(serializedProperty.isArray, Is.True);

            for (int i = 0; i < serializedProperty.arraySize; i++)
            {
                using SerializedProperty elementSerializedProperty = serializedProperty.GetArrayElementAtIndex(i);

                Assert.That(elementSerializedProperty, Is.Not.Null);

                using SerializedProperty relativeSerializedProperty = elementSerializedProperty.FindPropertyRelative(relativeProperty);

                Assert.That(relativeSerializedProperty, Is.Not.Null);
                GetValueWorks(serializedObject, relativeSerializedProperty, comparers.Select(x => x[i]).ToArray());
            }
        }

        private static void GetValueWorks<T>(SerializedObject serializedObject, SerializedProperty serializedProperty, IReadOnlyList<T> comparers)
        {
            UnityEngine.Object[] targets = serializedObject.targetObjects;
            object[] scopeArray = serializedProperty.GetValues();
            T[] scopeArrayT = serializedProperty.GetValues<T>();
            List<object> scopeList = new List<object>();
            List<T> scopeListT = new List<T>();
            serializedProperty.GetValues(scopeList);
            serializedProperty.GetValues(scopeListT);

            for (int i = 0; i < targets.Length; i++)
            {
                Assert.That(scopeArray[i], Is.EqualTo(comparers[i]));
                Assert.That(scopeArrayT[i], Is.EqualTo(comparers[i]));
                Assert.That(scopeList[i], Is.EqualTo(comparers[i]));
                Assert.That(scopeListT[i], Is.EqualTo(comparers[i]));
            }
        }

        private static void SetValueWorks<T>(T[] targets)
            where T : UnityEngine.Object, IDummyInterface
        {
            using SerializedObject serializedObject = new SerializedObject(targets.ToArray<UnityEngine.Object>());

            Assert.That(serializedObject, Is.Not.Null);
            SetValueWorks<int>(serializedObject, "_integer", x => int.MaxValue, (i, x) => targets[i].Integer == x);
            SetValueWorks<int[]>(serializedObject, "_integerArray", x => null, (i, x) => targets[i].IntegerArray == x);
            SetValueWorks<string>(serializedObject, "_string", x => string.Empty, (i, x) => targets[i].String == x);
            SetValueWorks<string[]>(serializedObject, "_stringArray", x => null, (i, x) => targets[i].StringArray == x);
            SetValueWorks<Vector3Int>(serializedObject, "_vector", x => Vector3Int.zero, (i, x) => targets[i].Vector == x);
            SetValueWorks<Vector3Int[]>(serializedObject, "_vectorArray", x => null, (i, x) => targets[i].VectorArray == x);
            SetValueWorks<int>(serializedObject, "_vector.x", x => int.MaxValue, (i, x) => targets[i].Vector.x == x);
            SetValueWorks<int>(serializedObject, "_vector.y", x => int.MaxValue, (i, x) => targets[i].Vector.y == x);
            SetValueWorks<int>(serializedObject, "_vector.z", x => int.MaxValue, (i, x) => targets[i].Vector.z == x);
            SetValueWorks<int>(serializedObject, "_vector", "x", x => int.MaxValue, (i, x) => targets[i].Vector.x == x);
            SetValueWorks<int>(serializedObject, "_vector", "y", x => int.MaxValue, (i, x) => targets[i].Vector.y == x);
            SetValueWorks<int>(serializedObject, "_vector", "z", x => int.MaxValue, (i, x) => targets[i].Vector.z == x);
        }

        private static void SetValueWorks<T>(SerializedObject serializedObject, string property, PropertyPathInfo.SetValueHandler<T> onSetValue, Func<int, T, bool> onValidate)
        {
            using SerializedProperty serializedProperty = serializedObject.FindProperty(property);

            Assert.That(serializedProperty, Is.Not.Null);
            SetValueWorks(serializedProperty, onSetValue, onValidate);
        }

        private static void SetValueWorks<T>(SerializedObject serializedObject, string property, string relativeProperty, PropertyPathInfo.SetValueHandler<T> onSetValue, Func<int, T, bool> onValidate)
        {
            using SerializedProperty serializedProperty = serializedObject.FindProperty(property);

            Assert.That(serializedProperty, Is.Not.Null);

            using SerializedProperty relativeSerializedProperty = serializedProperty.FindPropertyRelative(relativeProperty);

            Assert.That(relativeSerializedProperty, Is.Not.Null);
            SetValueWorks(relativeSerializedProperty, onSetValue, onValidate);
        }

        private static void SetValueWorks<T>(SerializedProperty serializedProperty, PropertyPathInfo.SetValueHandler<T> onSetValue, Func<int, T, bool> onValidate)
        {
            serializedProperty.SetValues(onSetValue);

            T[] values = serializedProperty.GetValues<T>();

            for (int i = 0; i < values.Length; i++)
            {
                T value = values[i];
                Assert.That(onValidate(i, value), Is.True);
            }
        }
    }
}
