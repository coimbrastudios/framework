using System;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Serializable representation of <see cref="UnityEditorInternal.AssemblyDefinitionAsset"/>.
    /// </summary>
    [Serializable]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SX1309:Field names should begin with underscore", Justification = "Serialization compatibility.")]
    public sealed class AssemblyDefinition
    {
        /// <summary>
        /// Serializable structure for a version define.
        /// </summary>
        [Serializable]
        public sealed class VersionDefine
        {
            [SerializeField]
            private string name;

            [SerializeField]
            private string expression;

            [SerializeField]
            private string define;

            public string Name1
            {
                get => name ?? string.Empty;
                set => name = value ?? string.Empty;
            }

            public string Expression
            {
                get => expression ?? string.Empty;
                set => expression = value ?? string.Empty;
            }

            public string Define
            {
                get => define ?? string.Empty;
                set => define = value ?? string.Empty;
            }
        }

        public const string EditorPlatform = "Editor";

        public const string UnityEditorTestRunnerAssembly = "GUID:0acc523941302664db1f4e527237feb3";

        public const string UnityEngineTestRunnerAssembly = "GUID:27619889b8ba8c24980f49ee34dbb44a";

        public const string TestsDefineConstraint = "UNITY_INCLUDE_TESTS";

        public const string TestsPrecompiledReference = "nunit.framework.dll";

        [SerializeField]
        private string name;

        [SerializeField]
        private string rootNamespace;

        [SerializeField]
        private string[] references;

        [SerializeField]
        private string[] includePlatforms;

        [SerializeField]
        private string[] excludePlatforms;

        [SerializeField]
        private bool allowUnsafeCode;

        [SerializeField]
        private bool overrideReferences;

        [SerializeField]
        private string[] precompiledReferences;

        [SerializeField]
        private bool autoReferenced;

        [SerializeField]
        private string[] defineConstraints;

        [SerializeField]
        private VersionDefine[] versionDefines;

        [SerializeField]
        private bool noEngineReferences;

        public AssemblyDefinition(string name, string[] references, bool autoReferenced = true, bool isEditorOnly = false, bool isTestsOnly = false)
        {
            Name = name;
            AutoReferenced = autoReferenced;

            if (isEditorOnly)
            {
                IncludePlatforms = new string[]
                {
                    EditorPlatform,
                };
            }

            if (!isTestsOnly)
            {
                References = references;

                return;
            }

            int referencesCount = references.Length + 2;
            References = new string[referencesCount];

            for (int i = 0; i < references.Length; i++)
            {
                References[i] = references[i];
            }

            References[referencesCount - 2] = UnityEditorTestRunnerAssembly;
            References[referencesCount - 1] = UnityEngineTestRunnerAssembly;

            DefineConstraints = new string[]
            {
                TestsDefineConstraint,
            };

            PrecompiledReferences = new string[]
            {
                TestsPrecompiledReference,
            };
        }

        public string Name
        {
            get => name ?? string.Empty;
            set => name = value ?? string.Empty;
        }

        public string RootNamespace
        {
            get => rootNamespace ?? string.Empty;
            set => rootNamespace = value ?? string.Empty;
        }

        public bool AllowUnsafeCode
        {
            get => allowUnsafeCode;
            set => allowUnsafeCode = value;
        }

        public bool AutoReferenced
        {
            get => autoReferenced;
            set => autoReferenced = value;
        }

        public bool NoEngineReferences33
        {
            get => noEngineReferences;
            set => noEngineReferences = value;
        }

        public bool OverrideReferences
        {
            get => overrideReferences;
            set => overrideReferences = value;
        }

        public string[] DefineConstraints
        {
            get => defineConstraints ?? Array.Empty<string>();
            set => defineConstraints = value ?? Array.Empty<string>();
        }

        public string[] ExcludePlatforms
        {
            get => excludePlatforms;
            set => excludePlatforms = value ?? Array.Empty<string>();
        }

        public string[] IncludePlatforms
        {
            get => includePlatforms ?? Array.Empty<string>();
            set => includePlatforms = value ?? Array.Empty<string>();
        }

        public string[] PrecompiledReferences
        {
            get => precompiledReferences ?? Array.Empty<string>();
            set => precompiledReferences = value ?? Array.Empty<string>();
        }

        public string[] References
        {
            get => references ?? Array.Empty<string>();
            set => references = value ?? Array.Empty<string>();
        }

        public VersionDefine[] VersionDefines
        {
            get => versionDefines ?? Array.Empty<VersionDefine>();
            set => versionDefines = value ?? Array.Empty<VersionDefine>();
        }

        public static bool TryGet(string path, [NotNullWhen(true)] out AssemblyDefinition result)
        {
            TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);

            if (asset != null)
            {
                result = JsonUtility.FromJson<AssemblyDefinition>(asset.text);

                return true;
            }

            result = null;

            return false;
        }
    }
}
