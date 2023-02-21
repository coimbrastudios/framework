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
    [ProjectSettings(CoimbraUtility.ProjectSettingsPath, true, FileDirectory = CoimbraUtility.ProjectSettingsFilePath)]
    public sealed class LintingSettings : ScriptableSettings
    {
        private const string AssemblyDefinitionFilter = "t:asmdef";

        private static readonly string[] DefaultRules =
        {
            "30f3d018aa91612458a73822b63c1733",
            "fbf9a035505766a41b1f7fb8e54fbe15",
            "c118adc722ff7994c8bfe76332da9b3f",
            "bd97c2eb44693c44589fc7fbcde28a29",
            "ef0791cf148fdd9448284359b6b312ab",
            "9c9fc9d6ef1bad04b976c792302e8ed0",
            "58460b9a11c55a749924b05acd6efc4b",
            "08c8f835d645d9340b7def3a31fead74",
            "724079212532c9840a307b6a7fe9ba88",
            "edd3021720ab4a24ca46ffb39b2b7eb9",
            "3ba3cb4ef983c9a43b25adafc1757b84",
            "8785eb2eeddc1fd40b90de45916dc280",
            "29537581b2d432a44a3e12b2f8062bfd",
        };

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
            bool isDirty = false;
            LintingSettings settings = Get<LintingSettings>();

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

            if (!isDirty)
            {
                return;
            }

            Debug.Log("Applying assembly definition rules...");
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        /// <inheritdoc/>
        protected override void OnReset()
        {
            base.OnReset();
            AssemblyDefinitionRules.Clear();
            SetDefaultRules();
            Save();
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

        private void SetDefaultRules()
        {
            foreach (string guid in DefaultRules)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                AssemblyDefinitionRuleBase rule = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionRuleBase>(path);

                if (rule != null)
                {
                    AssemblyDefinitionRules.Add(rule);
                }
                else
                {
                    Debug.LogError($"Missing asset {guid} at path: {guid}", rule);
                }
            }
        }
    }
}
