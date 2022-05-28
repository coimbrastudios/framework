using System;
using UnityEngine;

namespace Coimbra.Editor
{
    [Serializable]
    internal sealed class AssemblyDefinition
    {
        [SerializeField]
        internal string name;

        [SerializeField]
        internal bool autoReferenced;

        [SerializeField]
        internal string[] references;

        [SerializeField]
        internal string[] includePlatforms;

        [SerializeField]
        internal string[] defineConstraints;

        internal AssemblyDefinition(string name, string[] references, bool isEditorOnly)
        {
            this.name = name;
            this.references = references;
            autoReferenced = true;

            if (isEditorOnly)
            {
                defineConstraints = new[]
                {
                    "UNITY_EDITOR"
                };

                includePlatforms = new[]
                {
                    "Editor"
                };
            }
            else
            {
                defineConstraints = Array.Empty<string>();
                includePlatforms = Array.Empty<string>();
            }
        }
    }
}
