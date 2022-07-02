#pragma warning disable SA1401
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coimbra.Tests
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

        public string[] StringArray = new string[5];

        public List<NestedClass.NestedStruct> StructList = new()
        {
            default,
        };

        internal const string FieldName = nameof(_field);

        [SerializeReference]
        internal NestedClass Reference = new();

        [SerializeField]
        private NestedClass _field;

        internal NestedClass Field
        {
            get => _field;
            set => _field = value;
        }
    }
}
#pragma warning restore SA1401
