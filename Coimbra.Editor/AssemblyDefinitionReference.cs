using System;
using UnityEngine;

namespace Coimbra.Editor
{
    [Serializable]
    internal sealed class AssemblyDefinitionReference
    {
        [SerializeField]
        internal string reference;

        internal AssemblyDefinitionReference(string reference)
        {
            this.reference = reference;
        }
    }
}
