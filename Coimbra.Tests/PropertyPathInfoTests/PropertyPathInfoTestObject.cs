using UnityEngine;

namespace Coimbra.Tests
{
    internal class PropertyPathInfoTestObject : ScriptableObject
    {
#pragma warning disable SA1401
        public float Float;

        public PropertyPathInfoTestBehaviourBase.NestedClass.NestedStruct OtherClassNestedStruct;
#pragma warning restore SA1401
    }
}
