using System;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Serializable representation of <see cref="UnityEditorInternal.AssemblyDefinitionReferenceAsset"/>.
    /// </summary>
    [Serializable]
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

        public static bool TryGet(string path, out AssemblyDefinitionReference result)
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
