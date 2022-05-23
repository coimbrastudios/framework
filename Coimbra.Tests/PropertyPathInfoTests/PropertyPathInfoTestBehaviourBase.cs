using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coimbra.Editor.Tests
{
    internal abstract class PropertyPathInfoTestBehaviourBase : MonoBehaviour
    {
        [Serializable]
        internal sealed class NestedClass
        {
            [Serializable]
            internal struct NestedStruct
            {
                [SerializeField]
                internal string String;
            }

            public int Integer;

            [SerializeField]
            internal NestedStruct NestedStructValue;
        }

        internal const string FieldName = nameof(_field);

        public string[] StringArray = new string[5];

        public List<NestedClass.NestedStruct> StructList = new List<NestedClass.NestedStruct>
        {
            default
        };

        [SerializeReference]
        internal NestedClass Reference = new NestedClass();

        [SerializeField]
        private NestedClass _field;

        internal NestedClass Field
        {
            get => _field;
            set => _field = value;
        }
    }
}
