using System.Collections.Generic;
using UnityEngine;

namespace Coimbra.Editor.Linting
{
    /// <summary>
    /// Fix assemblies that should only be compiled inside the editor.
    /// </summary>
    [CreateAssetMenu(menuName = CoimbraUtility.GeneralMenuPath + DefaultAssetMenuPath + "Fix Editor Only")]
    public sealed class FixEditorOnlyAssemblyDefinitionRule : AssemblyDefinitionRuleBase
    {
        private const string EditorDefine = "UNITY_EDITOR";

        private const string EditorPlatform = "Editor";

        /// <inheritdoc/>
        public override bool Apply(AssemblyDefinition assemblyDefinition, Object context)
        {
            // We want to run both always.
            return ApplyDefines(assemblyDefinition, context)
                 | ApplyPlatforms(assemblyDefinition, context);
        }

        private static bool ApplyDefines(AssemblyDefinition assemblyDefinition, Object context)
        {
            foreach (string define in assemblyDefinition.DefineConstraints)
            {
                if (define == EditorDefine)
                {
                    return false;
                }
            }

            using (ListPool.Pop(out List<string> defines))
            {
                defines.AddRange(assemblyDefinition.DefineConstraints);
                defines.Add(EditorDefine);
                assemblyDefinition.DefineConstraints = defines.ToArray();
            }

            Debug.LogWarning($"{assemblyDefinition.Name} was missing {EditorDefine} define constraint!", context);

            return true;
        }

        private static bool ApplyPlatforms(AssemblyDefinition assemblyDefinition, Object context)
        {
            if (assemblyDefinition.ExcludePlatforms.Length == 0
             && assemblyDefinition.IncludePlatforms.Length == 1
             && assemblyDefinition.IncludePlatforms[0] == EditorPlatform)
            {
                return false;
            }

            assemblyDefinition.ExcludePlatforms = null;

            assemblyDefinition.IncludePlatforms = new string[]
            {
                EditorPlatform,
            };

            Debug.LogWarning($"{assemblyDefinition.Name} had incorrect platforms, changed to only use {EditorPlatform}!", context);

            return true;
        }
    }
}
