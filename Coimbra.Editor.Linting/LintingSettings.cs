using JetBrains.Annotations;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Coimbra.Editor.Linting
{
    /// <summary>
    /// Settings for linting the project.
    /// </summary>
    [InitializeOnLoad]
    [ProjectSettings(CoimbraUtility.ProjectSettingsPath, true, FileDirectory = null)]
    public sealed class LintingSettings : ScriptableSettings
    {
        private const string AssemblyDefinitionFilter = "t:asmdef";

        static LintingSettings()
        {
            CompilationPipeline.assemblyCompilationFinished -= HandleAssemblyCompilationFinished;
            CompilationPipeline.assemblyCompilationFinished += HandleAssemblyCompilationFinished;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the local Unity analyzers will be disabled for the generated CS project. This is required for when using 'Microsoft.Unity.Analyzers' directly.
        /// </summary>
        [PublicAPI]
        [field: SerializeField]
        [field: Tooltip("If true, local analyzers will be disabled for the generated CS project. This is required for when using 'Microsoft.Unity.Analyzers' directly.")]
        public bool DisableLocalUnityAnalyzers { get; set; }

        /// <summary>
        /// Gets or sets collection of <see cref="AssemblyDefinitionRuleBase"/> to use project-wide.
        /// </summary>
        [DisallowNull]
        [field: SerializeField]
        [field: Tooltip("Collection of assembly definition rules to use project-wide.")]
        public List<AssemblyDefinitionRuleBase> AssemblyDefinitionRules { get; set; } = new();

        [InitializeOnLoadMethod]
        internal static void InitializeAssemblyDefinitionRules()
        {
            if (!TryGetOrFind(out LintingSettings settings))
            {
                return;
            }

            bool isDirty = false;

            using (DictionaryPool.Pop(out Dictionary<string, TextAsset> textAssetMap))
            using (DictionaryPool.Pop(out Dictionary<TextAsset, AssemblyDefinition> assemblyDefinitionMap))
            {
                foreach (string guid in AssetDatabase.FindAssets(AssemblyDefinitionFilter))
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                    foreach (AssemblyDefinitionRuleBase rule in settings.AssemblyDefinitionRules)
                    {
                        if (TryApply(rule, assetPath, textAssetMap, assemblyDefinitionMap))
                        {
                            isDirty = true;
                        }
                    }
                }
            }

            if (isDirty)
            {
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
        }

        private static void HandleAssemblyCompilationFinished(string assemblyPath, CompilerMessage[] compilerMessages)
        {
            foreach (CompilerMessage compilerMessage in compilerMessages)
            {
                if (compilerMessage.type == CompilerMessageType.Error)
                {
                    InitializeAssemblyDefinitionRules();
                }
            }
        }

        private static bool TryApply(AssemblyDefinitionRuleBase rule, in string assetPath, IDictionary<string, TextAsset> textAssetMap, IDictionary<TextAsset, AssemblyDefinition> assemblyDefinitionMap)
        {
            if (!rule.CanApply(assetPath) || !TryLoadAsset(assetPath, out TextAsset asset, textAssetMap))
            {
                return false;
            }

            if (!assemblyDefinitionMap.TryGetValue(asset, out AssemblyDefinition assembly))
            {
                assembly = JsonUtility.FromJson<AssemblyDefinition>(asset.text);
                assemblyDefinitionMap[asset] = assembly;
            }

            if (!rule.Apply(assembly, asset))
            {
                return false;
            }

            File.WriteAllText(assetPath, JsonUtility.ToJson(assembly, true));

            return true;
        }

        private static bool TryLoadAsset(in string assetPath, [NotNullWhen(true)] out TextAsset asset, IDictionary<string, TextAsset> cache)
        {
            if (cache.TryGetValue(assetPath, out asset))
            {
                return true;
            }

            asset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);

            if (asset != null)
            {
                cache.Add(assetPath, asset);

                return true;
            }

            Debug.LogError($"File ends with '.asmdef' but is not a {nameof(TextAsset)}: {assetPath}.");

            return false;
        }
    }
}
