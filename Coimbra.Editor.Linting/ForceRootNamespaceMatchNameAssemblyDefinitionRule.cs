using UnityEngine;

namespace Coimbra.Editor.Linting
{
    /// <summary>
    /// Force the root namespace of the assembly to match the assembly name.
    /// </summary>
    [CreateAssetMenu(menuName = CoimbraUtility.GeneralMenuPath + DefaultAssetMenuPath + "Force Root Namespace Match Name")]
    public sealed class ForceRootNamespaceMatchNameAssemblyDefinitionRule : AssemblyDefinitionRuleBase
    {
        /// <inheritdoc/>
        public override bool Apply(AssemblyDefinition assemblyDefinition, Object context)
        {
            if (assemblyDefinition.Name == assemblyDefinition.RootNamespace)
            {
                return false;
            }

            assemblyDefinition.RootNamespace = assemblyDefinition.Name;
            Debug.LogWarning($"{assemblyDefinition.Name} had a root namespace not matching its names!", context);

            return true;
        }
    }
}
