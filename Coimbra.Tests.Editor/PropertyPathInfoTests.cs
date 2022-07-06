using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Coimbra.Tests.Editor
{
    [TestFixture]
    [TestOf(typeof(PropertyPathInfo))]
    internal sealed class PropertyPathInfoTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            PropertyPathInfoUtility.ClearCaches();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            PropertyPathInfoUtility.ClearCaches();
        }

        [TestCase(0, nameof(PropertyPathInfoTestBehaviour.Integer))]
        [TestCase(2, nameof(PropertyPathInfoTestBehaviourBase.StructList) + ".Array.data[0]", nameof(PropertyPathInfoTestBehaviourBase.NestedClass.NestedStruct.String))]
        [TestCase(1, nameof(PropertyPathInfoTestBehaviourBase.StringArray) + ".Array.data[4]")]
        [TestCase(0, nameof(PropertyPathInfoTestBehaviourBase.StringArray))]
        [TestCase(2, nameof(PropertyPathInfoTestBehaviourBase.Reference), nameof(PropertyPathInfoTestBehaviourBase.NestedClass.NestedStructValue), nameof(PropertyPathInfoTestBehaviourBase.NestedClass.NestedStruct.String))]
        [TestCase(1, nameof(PropertyPathInfoTestBehaviourBase.Reference), nameof(PropertyPathInfoTestBehaviourBase.NestedClass.NestedStructValue))]
        [TestCase(0, nameof(PropertyPathInfoTestBehaviourBase.Reference))]
        [TestCase(2, PropertyPathInfoTestBehaviourBase.FieldName, nameof(PropertyPathInfoTestBehaviourBase.NestedClass.NestedStructValue), nameof(PropertyPathInfoTestBehaviourBase.NestedClass.NestedStruct.String))]
        [TestCase(1, PropertyPathInfoTestBehaviourBase.FieldName, nameof(PropertyPathInfoTestBehaviourBase.NestedClass.NestedStructValue))]
        [TestCase(0, PropertyPathInfoTestBehaviourBase.FieldName)]
        public void PropertyPathInfo_Depth_TestCases_TestBehaviour(int depth, params string[] propertyPath)
        {
            PropertyPathInfoTestBehaviour testBehaviour = new GameObject().AddComponent<PropertyPathInfoTestBehaviour>();

            try
            {
                using SerializedObject serializedObject = new(testBehaviour);
                using SerializedProperty serializedProperty = serializedObject.FindProperty(propertyPath[0]);
                Assert.That(serializedProperty, Is.Not.Null);

                SerializedProperty target = serializedProperty;

                for (int i = 1; i < propertyPath.Length; i++)
                {
                    target = target.FindPropertyRelative(propertyPath[i]);
                }

                Assert.That(target, Is.Not.Null);
                Assert.That(target.depth, Is.EqualTo(depth));

                PropertyPathInfo propertyPathInfo = serializedObject.targetObject.GetPropertyPathInfo(target.propertyPath);
                Assert.That(propertyPathInfo.Depth, Is.EqualTo(depth));
            }
            finally
            {
                Object.DestroyImmediate(testBehaviour.gameObject);
            }
        }

        [TestCase(1, nameof(PropertyPathInfoTestObject.OtherClassNestedStruct), nameof(PropertyPathInfoTestBehaviourBase.NestedClass.NestedStruct.String))]
        [TestCase(0, nameof(PropertyPathInfoTestObject.OtherClassNestedStruct))]
        [TestCase(0, nameof(PropertyPathInfoTestObject.Float))]
        public void PropertyPathInfo_Depth_TestCases_TestObject(int depth, params string[] propertyPath)
        {
            PropertyPathInfoTestObject testObject = ScriptableObject.CreateInstance<PropertyPathInfoTestObject>();

            try
            {
                using SerializedObject serializedObject = new(testObject);
                using SerializedProperty serializedProperty = serializedObject.FindProperty(propertyPath[0]);
                SerializedProperty target = serializedProperty;

                for (int i = 1; i < propertyPath.Length; i++)
                {
                    target = target.FindPropertyRelative(propertyPath[i]);
                }

                Assert.That(target.depth, Is.EqualTo(depth));

                PropertyPathInfo propertyPathInfo = serializedObject.targetObject.GetPropertyPathInfo(target.propertyPath);
                Assert.That(propertyPathInfo.Depth, Is.EqualTo(depth));
            }
            finally
            {
                Object.DestroyImmediate(testObject);
            }
        }

        [Test]
        public void PropertyPathInfo_GetScope_Depth0()
        {
            PropertyPathInfoTestBehaviour testBehaviour = new GameObject().AddComponent<PropertyPathInfoTestBehaviour>();

            try
            {
                using SerializedObject serializedObject = new(testBehaviour);
                using SerializedProperty serializedProperty = serializedObject.FindProperty(nameof(PropertyPathInfoTestBehaviour.Integer));
                Assert.That(serializedProperty, Is.Not.Null);

                PropertyPathInfo propertyPathInfo = serializedObject.targetObject.GetPropertyPathInfo(serializedProperty.propertyPath);
                Assert.That(propertyPathInfo.GetScope(serializedObject.targetObject), Is.EqualTo(testBehaviour));
            }
            finally
            {
                Object.DestroyImmediate(testBehaviour.gameObject);
            }
        }

        [Test]
        public void PropertyPathInfo_GetScope_Depth1_Array()
        {
            const int testSize = 3;

            PropertyPathInfoTestBehaviour testBehaviour = new GameObject().AddComponent<PropertyPathInfoTestBehaviour>();
            testBehaviour.StringArray = new string[testSize];

            try
            {
                using SerializedObject serializedObject = new(testBehaviour);
                using SerializedProperty serializedProperty = serializedObject.FindProperty(nameof(PropertyPathInfoTestBehaviourBase.StringArray));
                Assert.That(serializedProperty, Is.Not.Null);

                PropertyPathInfo propertyPathInfo = serializedObject.targetObject.GetPropertyPathInfo(serializedProperty.propertyPath);
                Assert.That(propertyPathInfo.GetScope(serializedObject.targetObject), Is.EqualTo(testBehaviour));
            }
            finally
            {
                Object.DestroyImmediate(testBehaviour.gameObject);
            }
        }

        [Test]
        public void PropertyPathInfo_GetScope_Depth1_Field()
        {
            PropertyPathInfoTestBehaviour testBehaviour = new GameObject().AddComponent<PropertyPathInfoTestBehaviour>();
            testBehaviour.Field = new PropertyPathInfoTestBehaviourBase.NestedClass();

            try
            {
                using SerializedObject serializedObject = new(testBehaviour);
                using SerializedProperty serializedProperty = serializedObject.FindProperty($"{PropertyPathInfoTestBehaviourBase.FieldName}.{nameof(PropertyPathInfoTestBehaviourBase.NestedClass.Integer)}");
                Assert.That(serializedProperty, Is.Not.Null);

                PropertyPathInfo propertyPathInfo = serializedObject.targetObject.GetPropertyPathInfo(serializedProperty.propertyPath);
                Assert.That(propertyPathInfo.GetScope(serializedObject.targetObject), Is.EqualTo(testBehaviour.Field));
            }
            finally
            {
                Object.DestroyImmediate(testBehaviour.gameObject);
            }
        }

        [Test]
        public void PropertyPathInfo_GetScope_Depth2_Array()
        {
            const int testSize = 10;
            const int testIndex = 3;

            string testPath = $"{nameof(PropertyPathInfoTestBehaviourBase.StructList)}.Array.data[{testIndex}].{nameof(PropertyPathInfoTestBehaviourBase.NestedClass.NestedStruct.String)}";
            PropertyPathInfoTestBehaviour testBehaviour = new GameObject().AddComponent<PropertyPathInfoTestBehaviour>();
            testBehaviour.StructList.AddRange(new PropertyPathInfoTestBehaviourBase.NestedClass.NestedStruct[testSize]);
            testBehaviour.StructList[testIndex] = default;

            try
            {
                using SerializedObject serializedObject = new(testBehaviour);
                using SerializedProperty serializedProperty = serializedObject.FindProperty(testPath);
                Assert.That(serializedProperty, Is.Not.Null);

                PropertyPathInfo propertyPathInfo = serializedObject.targetObject.GetPropertyPathInfo(serializedProperty.propertyPath);
                Assert.That(propertyPathInfo.GetScope(serializedObject.targetObject), Is.EqualTo(testBehaviour.StructList));
            }
            finally
            {
                Object.DestroyImmediate(testBehaviour.gameObject);
            }
        }

        [Test]
        public void PropertyPathInfo_GetScope_Depth2_Reference()
        {
            string testPath = $"{nameof(PropertyPathInfoTestBehaviourBase.Reference)}.{nameof(PropertyPathInfoTestBehaviourBase.NestedClass.NestedStructValue)}.{nameof(PropertyPathInfoTestBehaviourBase.NestedClass.NestedStruct.String)}";
            PropertyPathInfoTestBehaviour testBehaviour = new GameObject().AddComponent<PropertyPathInfoTestBehaviour>();

            testBehaviour.Reference = new PropertyPathInfoTestBehaviourBase.NestedClass
            {
                NestedStructValue = default,
            };

            try
            {
                using SerializedObject serializedObject = new(testBehaviour);
                using SerializedProperty serializedProperty = serializedObject.FindProperty(testPath);
                Assert.That(serializedProperty, Is.Not.Null);

                PropertyPathInfo propertyPathInfo = serializedObject.targetObject.GetPropertyPathInfo(serializedProperty.propertyPath);
                Assert.That(propertyPathInfo.GetScope(serializedObject.targetObject), Is.EqualTo(testBehaviour.Reference.NestedStructValue));
            }
            finally
            {
                Object.DestroyImmediate(testBehaviour.gameObject);
            }
        }

        [Test]
        public void PropertyPathInfo_GetValue_Depth0()
        {
            const int testInt = 42;

            PropertyPathInfoTestBehaviour testBehaviour = new GameObject().AddComponent<PropertyPathInfoTestBehaviour>();
            testBehaviour.Integer = testInt;

            try
            {
                using SerializedObject serializedObject = new(testBehaviour);
                using SerializedProperty serializedProperty = serializedObject.FindProperty(nameof(PropertyPathInfoTestBehaviour.Integer));
                Assert.That(serializedProperty, Is.Not.Null);

                PropertyPathInfo propertyPathInfo = serializedObject.targetObject.GetPropertyPathInfo(serializedProperty.propertyPath);
                Assert.That(propertyPathInfo.GetValue(serializedObject.targetObject), Is.EqualTo(testInt));
            }
            finally
            {
                Object.DestroyImmediate(testBehaviour.gameObject);
            }
        }

        [Test]
        public void PropertyPathInfo_GetValue_Depth1_Array()
        {
            const int testSize = 3;
            const int testIndex = 1;
            const string testString = "Test";

            PropertyPathInfoTestBehaviour testBehaviour = new GameObject().AddComponent<PropertyPathInfoTestBehaviour>();
            testBehaviour.StringArray = new string[testSize];
            testBehaviour.StringArray[testIndex] = testString;

            try
            {
                using SerializedObject serializedObject = new(testBehaviour);
                using SerializedProperty serializedProperty = serializedObject.FindProperty(nameof(PropertyPathInfoTestBehaviourBase.StringArray));
                Assert.That(serializedProperty, Is.Not.Null);

                PropertyPathInfo propertyPathInfo = serializedObject.targetObject.GetPropertyPathInfo(serializedProperty.propertyPath);
                string[] stringArray = propertyPathInfo.GetValue(serializedObject.targetObject) as string[];
                Assert.That(stringArray, Is.Not.Null);
                Assert.That(stringArray.Length, Is.EqualTo(testSize));
                Assert.That(stringArray[testIndex], Is.EqualTo(testString));
            }
            finally
            {
                Object.DestroyImmediate(testBehaviour.gameObject);
            }
        }

        [Test]
        public void PropertyPathInfo_GetValue_Depth1_Field()
        {
            const int testInt = 42;

            PropertyPathInfoTestBehaviour testBehaviour = new GameObject().AddComponent<PropertyPathInfoTestBehaviour>();

            testBehaviour.Field = new PropertyPathInfoTestBehaviourBase.NestedClass
            {
                Integer = testInt,
            };

            try
            {
                using SerializedObject serializedObject = new(testBehaviour);
                using SerializedProperty serializedProperty = serializedObject.FindProperty($"{PropertyPathInfoTestBehaviourBase.FieldName}.{nameof(PropertyPathInfoTestBehaviourBase.NestedClass.Integer)}");
                Assert.That(serializedProperty, Is.Not.Null);

                PropertyPathInfo propertyPathInfo = serializedObject.targetObject.GetPropertyPathInfo(serializedProperty.propertyPath);
                Assert.That(propertyPathInfo.GetValue(serializedObject.targetObject), Is.EqualTo(testInt));
            }
            finally
            {
                Object.DestroyImmediate(testBehaviour.gameObject);
            }
        }

        [Test]
        public void PropertyPathInfo_GetValue_Depth2_Array()
        {
            const int testSize = 10;
            const int testIndex = 3;
            const string testString = "Test";

            string testPath = $"{nameof(PropertyPathInfoTestBehaviourBase.StructList)}.Array.data[{testIndex}].{nameof(PropertyPathInfoTestBehaviourBase.NestedClass.NestedStruct.String)}";
            PropertyPathInfoTestBehaviour testBehaviour = new GameObject().AddComponent<PropertyPathInfoTestBehaviour>();
            testBehaviour.StructList.AddRange(new PropertyPathInfoTestBehaviourBase.NestedClass.NestedStruct[testSize]);

            testBehaviour.StructList[testIndex] = new PropertyPathInfoTestBehaviourBase.NestedClass.NestedStruct
            {
                String = testString,
            };

            try
            {
                using SerializedObject serializedObject = new(testBehaviour);
                using SerializedProperty serializedProperty = serializedObject.FindProperty(testPath);
                Assert.That(serializedProperty, Is.Not.Null);

                PropertyPathInfo propertyPathInfo = serializedObject.targetObject.GetPropertyPathInfo(serializedProperty.propertyPath);
                Assert.That(propertyPathInfo.GetValue(serializedObject.targetObject), Is.EqualTo(testString));
            }
            finally
            {
                Object.DestroyImmediate(testBehaviour.gameObject);
            }
        }

        [Test]
        public void PropertyPathInfo_GetValue_Depth2_Reference()
        {
            const string testString = "Test";

            string testPath = $"{nameof(PropertyPathInfoTestBehaviourBase.Reference)}.{nameof(PropertyPathInfoTestBehaviourBase.NestedClass.NestedStructValue)}.{nameof(PropertyPathInfoTestBehaviourBase.NestedClass.NestedStruct.String)}";
            PropertyPathInfoTestBehaviour testBehaviour = new GameObject().AddComponent<PropertyPathInfoTestBehaviour>();

            testBehaviour.Reference = new PropertyPathInfoTestBehaviourBase.NestedClass
            {
                NestedStructValue = new PropertyPathInfoTestBehaviourBase.NestedClass.NestedStruct
                {
                    String = testString,
                },
            };

            try
            {
                using SerializedObject serializedObject = new(testBehaviour);
                using SerializedProperty serializedProperty = serializedObject.FindProperty(testPath);
                Assert.That(serializedProperty, Is.Not.Null);

                PropertyPathInfo propertyPathInfo = serializedObject.targetObject.GetPropertyPathInfo(serializedProperty.propertyPath);
                Assert.That(propertyPathInfo.GetValue(serializedObject.targetObject), Is.EqualTo(testString));
            }
            finally
            {
                Object.DestroyImmediate(testBehaviour.gameObject);
            }
        }

        [Test]
        public void PropertyPathInfo_HasMultipleDifferentValues_ReturnsFalse()
        {
            const int testSize = 10;
            const int testIndex = 3;

            string testPath = $"{nameof(PropertyPathInfoTestBehaviourBase.StructList)}.Array.data[{testIndex}].{nameof(PropertyPathInfoTestBehaviourBase.NestedClass.NestedStruct.String)}";
            PropertyPathInfoTestBehaviour testBehaviour1 = new GameObject().AddComponent<PropertyPathInfoTestBehaviour>();
            PropertyPathInfoTestBehaviour testBehaviour2 = new GameObject().AddComponent<PropertyPathInfoTestBehaviour>();
            testBehaviour1.StructList.AddRange(new PropertyPathInfoTestBehaviourBase.NestedClass.NestedStruct[testSize]);
            testBehaviour2.StructList.AddRange(new PropertyPathInfoTestBehaviourBase.NestedClass.NestedStruct[testSize]);

            try
            {
                using SerializedObject serializedObject = new(new Object[]
                {
                    testBehaviour1,
                    testBehaviour2,
                });

                using SerializedProperty serializedProperty = serializedObject.FindProperty(testPath);
                Assert.That(serializedProperty, Is.Not.Null);

                PropertyPathInfo propertyPathInfo = serializedObject.targetObject.GetPropertyPathInfo(serializedProperty.propertyPath);
                Assert.That(propertyPathInfo.HasMultipleDifferentValues(serializedObject.targetObjects), Is.False);
            }
            finally
            {
                Object.DestroyImmediate(testBehaviour1.gameObject);
                Object.DestroyImmediate(testBehaviour2.gameObject);
            }
        }

        [Test]
        public void PropertyPathInfo_HasMultipleDifferentValues_ReturnsTrue()
        {
            const int testSize = 10;
            const int testIndex = 3;

            string testPath = $"{nameof(PropertyPathInfoTestBehaviourBase.StructList)}.Array.data[{testIndex}].{nameof(PropertyPathInfoTestBehaviourBase.NestedClass.NestedStruct.String)}";
            PropertyPathInfoTestBehaviour testBehaviour1 = new GameObject().AddComponent<PropertyPathInfoTestBehaviour>();
            PropertyPathInfoTestBehaviour testBehaviour2 = new GameObject().AddComponent<PropertyPathInfoTestBehaviour>();
            testBehaviour1.StructList.AddRange(new PropertyPathInfoTestBehaviourBase.NestedClass.NestedStruct[testSize]);
            testBehaviour2.StructList.AddRange(new PropertyPathInfoTestBehaviourBase.NestedClass.NestedStruct[testSize]);

            testBehaviour1.StructList[testIndex] = new PropertyPathInfoTestBehaviourBase.NestedClass.NestedStruct
            {
                String = "1",
            };

            testBehaviour2.StructList[testIndex] = new PropertyPathInfoTestBehaviourBase.NestedClass.NestedStruct
            {
                String = "2",
            };

            try
            {
                using SerializedObject serializedObject = new(new Object[]
                {
                    testBehaviour1,
                    testBehaviour2,
                });

                using SerializedProperty serializedProperty = serializedObject.FindProperty(testPath);
                Assert.That(serializedProperty, Is.Not.Null);

                PropertyPathInfo propertyPathInfo = serializedObject.targetObject.GetPropertyPathInfo(serializedProperty.propertyPath);
                Assert.That(propertyPathInfo.HasMultipleDifferentValues(serializedObject.targetObjects), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(testBehaviour1.gameObject);
                Object.DestroyImmediate(testBehaviour2.gameObject);
            }
        }

        [Test]
        public void PropertyPathInfo_SetValue()
        {
            const int testSize = 10;
            const int testIndex = 3;
            const string testString = "Test";

            string testPath = $"{nameof(PropertyPathInfoTestBehaviourBase.StructList)}.Array.data[{testIndex}].{nameof(PropertyPathInfoTestBehaviourBase.NestedClass.NestedStruct.String)}";
            PropertyPathInfoTestBehaviour testBehaviour = new GameObject("1").AddComponent<PropertyPathInfoTestBehaviour>();
            testBehaviour.StructList.AddRange(new PropertyPathInfoTestBehaviourBase.NestedClass.NestedStruct[testSize]);

            try
            {
                using SerializedObject serializedObject = new(testBehaviour);
                using SerializedProperty serializedProperty = serializedObject.FindProperty(testPath);
                Assert.That(serializedProperty, Is.Not.Null);

                PropertyPathInfo propertyPathInfo = serializedObject.targetObject.GetPropertyPathInfo(serializedProperty.propertyPath);
                propertyPathInfo.SetValue(serializedObject.targetObject, testString);
                Assert.That(propertyPathInfo.GetValue(serializedObject.targetObject), Is.EqualTo(testString));
            }
            finally
            {
                Object.DestroyImmediate(testBehaviour.gameObject);
            }
        }

        [Test]
        public void PropertyPathInfo_SetValues()
        {
            const int testSize = 10;
            const int testIndex = 3;
            const string testString = "Test";

            string testPath = $"{nameof(PropertyPathInfoTestBehaviourBase.StructList)}.Array.data[{testIndex}].{nameof(PropertyPathInfoTestBehaviourBase.NestedClass.NestedStruct.String)}";
            PropertyPathInfoTestBehaviour testBehaviour1 = new GameObject("1").AddComponent<PropertyPathInfoTestBehaviour>();
            PropertyPathInfoTestBehaviour testBehaviour2 = new GameObject("2").AddComponent<PropertyPathInfoTestBehaviour>();
            testBehaviour1.StructList.AddRange(new PropertyPathInfoTestBehaviourBase.NestedClass.NestedStruct[testSize]);
            testBehaviour2.StructList.AddRange(new PropertyPathInfoTestBehaviourBase.NestedClass.NestedStruct[testSize]);

            try
            {
                using SerializedObject serializedObject = new(new Object[]
                {
                    testBehaviour1,
                    testBehaviour2,
                });

                using SerializedProperty serializedProperty = serializedObject.FindProperty(testPath);
                Assert.That(serializedProperty, Is.Not.Null);

                PropertyPathInfo propertyPathInfo = serializedObject.targetObject.GetPropertyPathInfo(serializedProperty.propertyPath);
                Object[] targets = serializedObject.targetObjects;

                propertyPathInfo.SetValues(targets, false, delegate(PropertyPathInfo sender, Object target)
                {
                    return $"{testString}{target.name}";
                });

                object[] values = propertyPathInfo.GetValues(targets);

                for (int i = 0; i < values.Length; i++)
                {
                    Assert.That(values[i], Is.EqualTo($"{testString}{targets[i].name}"));
                }
            }
            finally
            {
                Object.DestroyImmediate(testBehaviour1.gameObject);
                Object.DestroyImmediate(testBehaviour2.gameObject);
            }
        }
    }
}
