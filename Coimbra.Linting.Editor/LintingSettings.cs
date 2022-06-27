﻿using Coimbra.Editor;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Linting.Editor
{
    /// <summary>
    /// Settings for linting the project.
    /// </summary>
    [ProjectSettings(CoimbraUtility.ProjectSettingsPath, true)]
    public sealed class LintingSettings : ScriptableSettings
    {
        private const string Filter = "t:asmdef";

        private static readonly string[] AssetsFolderOnly =
        {
            "Assets"
        };

        /// <summary>
        /// If true, local analyzers will be disabled for the generated CS project. This is required for when using 'Microsoft.Unity.Analyzers' directly.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("If true, local analyzers will be disabled for the generated CS project. This is required for when using 'Microsoft.Unity.Analyzers' directly.")]
        public bool DisableLocalUnityAnalyzers { get; set; } = false;

        /// <summary>
        /// If false, it will ignore Packages assemblies.;
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("If false, it will ignore Packages assemblies.")]
        public bool ApplyAssemblyDefinitionRulesInPackages { get; set; } = false;

        /// <summary>
        /// Collection of <see cref="AssemblyDefinitionRuleBase"/> to use project-wide.
        /// </summary>
        [NotNull]
        [field: SerializeField]
        [field: Tooltip("Collection of assembly definition rules to use project-wide.")]
        public List<AssemblyDefinitionRuleBase> AssemblyDefinitionRules { get; set; } = new List<AssemblyDefinitionRuleBase>();

        [InitializeOnLoadMethod]
        internal static void InitializeAssemblyDefinitionRules()
        {
            ScriptableSettingsUtility.TryLoadOrCreate(out LintingSettings settings, FindSingle);
            Debug.Assert(settings);

            string[] guids = settings.ApplyAssemblyDefinitionRulesInPackages ? AssetDatabase.FindAssets(Filter) : AssetDatabase.FindAssets(Filter, AssetsFolderOnly);

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                foreach (AssemblyDefinitionRuleBase rule in settings.AssemblyDefinitionRules)
                {
                    if (!rule.CanApply(assetPath))
                    {
                        continue;
                    }

                    TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);

                    if (asset == null)
                    {
                        Debug.LogError($"File ends with '.asmdef' but is not a {nameof(TextAsset)}: {assetPath}.");

                        continue;
                    }

                    AssemblyDefinition assembly = JsonUtility.FromJson<AssemblyDefinition>(asset.text);

                    if (!rule.Apply(assembly))
                    {
                        continue;
                    }

                    File.WriteAllText(assetPath, JsonUtility.ToJson(assembly, true));
                }
            }

            AssetDatabase.Refresh();
        }
    }
}