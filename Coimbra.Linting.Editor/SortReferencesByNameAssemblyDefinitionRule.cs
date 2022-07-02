using Coimbra.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Coimbra.Linting.Editor
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

            int IComparer<string>.Compare(string x, string y)
            {
                x = x!.StartsWith(GuidPrefix) ? x.Substring(GuidPrefix.Length) : x;
                y = y!.StartsWith(GuidPrefix) ? y.Substring(GuidPrefix.Length) : y;

                return StringComparer.Compare(Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(x)), Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(y)));
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
