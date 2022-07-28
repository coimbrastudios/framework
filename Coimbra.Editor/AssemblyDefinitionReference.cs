using System;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Serializable representation of <see cref="UnityEditorInternal.AssemblyDefinitionReferenceAsset"/>.
    /// </summary>
    [Serializable]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SX1309:Field names should begin with underscore", Justification = "Serialization compatibility.")]
    public sealed class AssemblyDefinitionReference
    {
        [SerializeField]
        private string reference;

        public AssemblyDefinitionReference(string reference)
        {
            Reference = reference;
        }

        public string Reference
        {
            get => reference ?? string.Empty;
            set => reference = value ?? string.Empty;
        }

        public static bool TryGet(string path, [NotNullWhen(true)] out AssemblyDefinitionReference result)
        {
            TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);

            if (asset != null)
            {
                result = JsonUtility.FromJson<AssemblyDefinitionReference>(asset.text);

                return true;
            }

            result = null;

            return false;
        }
    }
}
