using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Coimbra.Editor.Linting
{
    /// <summary>
    /// Sort all precompiled references alphabetically.
    /// </summary>
    [CreateAssetMenu(menuName = CoimbraUtility.GeneralMenuPath + DefaultAssetMenuPath + "Sort Precompiled References")]
    public sealed class SortPrecompiledReferencesAssemblyDefinitionRule : AssemblyDefinitionRuleBase
    {
        /// <inheritdoc/>
        public override bool Apply(AssemblyDefinition assemblyDefinition, Object context)
        {
            using (ListPool.Pop(out List<string> previous))
            {
                previous.AddRange(assemblyDefinition.PrecompiledReferences);
                Array.Sort(assemblyDefinition.PrecompiledReferences, StringComparer.InvariantCulture);

                for (int i = 0; i < previous.Count; i++)
                {
                    if (previous[i] == assemblyDefinition.PrecompiledReferences[i])
                    {
                        continue;
                    }

                    Debug.LogWarning($"{assemblyDefinition.Name} needed to sort its precompiled references!", context);

                    return true;
                }

                return false;
            }
        }
    }
}
