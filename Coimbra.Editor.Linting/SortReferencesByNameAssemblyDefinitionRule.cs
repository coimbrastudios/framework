using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Coimbra.Editor.Linting
{
    /// <summary>
    /// Sort all assembly definition references by their name.
    /// </summary>
    [CreateAssetMenu(menuName = CoimbraUtility.GeneralMenuPath + DefaultAssetMenuPath + "Sort References By Name")]
    public sealed class SortReferencesByNameAssemblyDefinitionRule : AssemblyDefinitionRuleBase
    {
        private sealed class GuidComparer : IComparer<string>
        {
            internal static readonly GuidComparer Instance = new();

            private const string GuidPrefix = "GUID:";

            private static readonly StringComparer StringComparer = StringComparer.InvariantCulture;

            private static void Normalize(ref string value)
            {
                if (!value.StartsWith(GuidPrefix))
                {
                    return;
                }

                value = value[GuidPrefix.Length..];
                value = Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(value));
            }

            int IComparer<string>.Compare(string x, string y)
            {
                Normalize(ref x);
                Normalize(ref y);

                return StringComparer.Compare(x, y);
            }
        }

        /// <inheritdoc/>
        public override bool Apply(AssemblyDefinition assemblyDefinition, Object context)
        {
            using (ListPool.Pop(out List<string> previous))
            {
                previous.AddRange(assemblyDefinition.References);
                Array.Sort(assemblyDefinition.References, GuidComparer.Instance);

                for (int i = 0; i < previous.Count; i++)
                {
                    if (previous[i] == assemblyDefinition.References[i])
                    {
                        continue;
                    }

                    Debug.LogWarning($"{assemblyDefinition.Name} needed to sort its references by name!", context);

                    return true;
                }

                return false;
            }
        }
    }
}
