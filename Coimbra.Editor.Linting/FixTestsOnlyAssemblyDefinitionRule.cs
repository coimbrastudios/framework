using System.Collections.Generic;
using UnityEngine;

namespace Coimbra.Editor.Linting
{
    /// <summary>
    /// Fix assemblies that should only be compiled for tests.
    /// </summary>
    [CreateAssetMenu(menuName = CoimbraUtility.GeneralMenuPath + DefaultAssetMenuPath + "Fix Tests Only")]
    public sealed class FixTestsOnlyAssemblyDefinitionRule : AssemblyDefinitionRuleBase
    {
        private const string TestsDefine = "UNITY_INCLUDE_TESTS";

        private const string TestsPrecompiledReference = "nunit.framework.dll";

        /// <inheritdoc/>
        public override bool Apply(AssemblyDefinition assemblyDefinition, Object context)
        {
            // We want to run both always.
            return ApplyDefines(assemblyDefinition, context)
                 | ApplyPrecompiledReferences(assemblyDefinition, context);
        }

        private static bool ApplyDefines(AssemblyDefinition assemblyDefinition, Object context)
        {
            foreach (string define in assemblyDefinition.DefineConstraints)
            {
                if (define == TestsDefine)
                {
                    return false;
                }
            }

            using (ListPool.Pop(out List<string> defines))
            {
                defines.AddRange(assemblyDefinition.DefineConstraints);
                defines.Add(TestsDefine);
                assemblyDefinition.DefineConstraints = defines.ToArray();
            }

            Debug.LogWarning($"{assemblyDefinition.Name} was missing {TestsDefine} define constraint!", context);

            return true;
        }

        private static bool ApplyPrecompiledReferences(AssemblyDefinition assemblyDefinition, Object context)
        {
            foreach (string define in assemblyDefinition.PrecompiledReferences)
            {
                if (define == TestsPrecompiledReference)
                {
                    return false;
                }
            }

            using (ListPool.Pop(out List<string> defines))
            {
                defines.AddRange(assemblyDefinition.PrecompiledReferences);
                defines.Add(TestsPrecompiledReference);
                assemblyDefinition.PrecompiledReferences = defines.ToArray();
            }

            Debug.LogWarning($"{assemblyDefinition.Name} was missing {TestsPrecompiledReference} precompiled reference!", context);

            return true;
        }
    }
}
