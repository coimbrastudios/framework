using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor.Linting
{
    /// <summary>
    /// Forces to use the assembly definition name directly instead of the GUID.
    /// </summary>
    [CreateAssetMenu(menuName = CoimbraUtility.GeneralMenuPath + DefaultAssetMenuPath + "Force Reference By Name")]
    public sealed class ForceReferenceByNameAssemblyDefinitionRule : AssemblyDefinitionRuleBase
    {
        /// <inheritdoc/>
        public override bool Apply(AssemblyDefinition assemblyDefinition, Object context)
        {
            const string prefix = "GUID:";
            bool result = false;

            for (int i = 0; i < assemblyDefinition.References.Length; i++)
            {
                if (!assemblyDefinition.References[i].StartsWith(prefix))
                {
                    continue;
                }

                string guid = assemblyDefinition.References[i][prefix.Length..];
                string path = AssetDatabase.GUIDToAssetPath(guid);
                TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);

                if (asset == null)
                {
                    continue;
                }

                string text = asset.text;
                AssemblyDefinition assembly = JsonUtility.FromJson<AssemblyDefinition>(text);
                assemblyDefinition.References[i] = assembly.Name;
                result = true;
                Debug.LogWarning($"{assemblyDefinition.Name} had reference by GUID to {assembly.Name}!", context);
            }

            return result;
        }
    }
}
