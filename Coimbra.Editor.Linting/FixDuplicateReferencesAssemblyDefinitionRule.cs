using System.Collections.Generic;
using UnityEngine;

namespace Coimbra.Editor.Linting
{
    /// <summary>
    /// Fix assemblies containing duplicate references.
    /// </summary>
    [CreateAssetMenu(menuName = CoimbraUtility.GeneralMenuPath + DefaultAssetMenuPath + "Fix Duplicate References")]
    public sealed class FixDuplicateReferencesAssemblyDefinitionRule : AssemblyDefinitionRuleBase
    {
        /// <inheritdoc/>
        public override bool Apply(AssemblyDefinition assemblyDefinition, Object context)
        {
            using (ListPool.Pop(out List<string> referencesList))
            using (HashSetPool.Pop(out HashSet<string> referencesSet))
            {
                string[] references = assemblyDefinition.References;
                referencesList.EnsureCapacity(references.Length);

                for (int i = 0; i < references.Length; i++)
                {
                    if (referencesSet.Add(references[i]))
                    {
                        referencesList.Add(references[i]);

                        continue;
                    }

                    Debug.LogWarning($"{assemblyDefinition.Name} had duplicate reference to {references[i]}!", context);
                }

                if (referencesList.Count == references.Length)
                {
                    return false;
                }

                assemblyDefinition.References = referencesList.ToArray();

                return true;
            }
        }
    }
}
