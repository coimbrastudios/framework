using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace Coimbra.Editor.Linting
{
    /// <summary>
    /// Base class to define a new rule for any <see cref="AssemblyDefinitionAsset"/>.
    /// </summary>
    public abstract class AssemblyDefinitionRuleBase : ScriptableObject
    {
        /// <summary>
        /// Default asset menu path to use with inherited types.
        /// </summary>
        protected const string DefaultAssetMenuPath = "Assembly Definition Rule/";

        [SerializeField]
        [FormerlySerializedAs("_includedMask")]
        [Tooltip("The list of targets to apply this rule. It accepts any path relative to the project folder. If empty it will include everything by default.")]
        private string[] _includedPatterns = Array.Empty<string>();

        [SerializeField]
        [FormerlySerializedAs("_excludedMask")]
        [Tooltip("The list of exceptions in the Included Patterns to not apply this rule. It accepts any path relative to the project folder. If empty it will not exclude anything by default.")]
        private string[] _excludedPatterns = Array.Empty<string>();

        [SerializeField]
        [UsedImplicitly]
        [HideInInspector]
        private bool _displayError;

        private bool _hasCaches;

        private Regex[] _excludedRegexes;

        private Regex[] _includedRegexes;

        /// <summary>
        /// Gets or sets the list of exceptions in the <see cref="IncludedPatterns"/> to not apply this rule. It accepts any path relative to the project folder. If empty it will not exclude anything by default.
        /// </summary>
        [NotNull]
        public IReadOnlyList<string> ExcludedPatterns
        {
            get => _excludedPatterns;
            set
            {
                _excludedPatterns = value.ToArray();
                OnValidate();
            }
        }

        /// <summary>
        /// Gets or sets the list of targets to apply this rule. It accepts any path relative to the project folder. If empty it will include everything by default.
        /// </summary>
        [NotNull]
        public IReadOnlyList<string> IncludedPatterns
        {
            get => _includedPatterns;
            set
            {
                _includedPatterns = value.ToArray();
                OnValidate();
            }
        }

        /// <summary>
        /// Implement this method to modify the given <paramref name="assemblyDefinition"/>.
        /// </summary>
        /// <returns>True if the assembly definition was actually modified, false otherwise.</returns>
        public abstract bool Apply(AssemblyDefinition assemblyDefinition, Object context);

        internal bool CanApply(string assemblyDefinitionPath)
        {
            InitializeCaches();

            if (_includedRegexes.Length > 0)
            {
                return HasAnyMatch(_includedRegexes, assemblyDefinitionPath) && !HasAnyMatch(_excludedRegexes, assemblyDefinitionPath);
            }

            if (_excludedRegexes.Length > 0)
            {
                return !HasAnyMatch(_excludedRegexes, assemblyDefinitionPath);
            }

            Debug.LogError("Rule should always have at least one mask (either included or excluded).", this);

            return false;
        }

        /// <summary>
        /// Helper method to build an array of regexes based on a list of masks.
        /// </summary>
        /// <seealso cref="PathUtility.GetRegexFromPattern"/>
        protected static Regex[] BuildRegexesCache(IReadOnlyList<string> patterns, bool ignoreCase)
        {
            Regex[] cache = new Regex[patterns.Count];

            for (int i = 0; i < cache.Length; i++)
            {
                cache[i] = PathUtility.GetRegexFromPattern(patterns[i], ignoreCase);
            }

            return cache;
        }

        /// <summary>
        /// Helper method to get the path for an assembly definition asset reference using either its guid or its name.
        /// </summary>
        /// <param name="reference">The GUID or name of the reference. If a GUID, it should start with "GUID:".</param>
        /// <returns>The asset path.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the asset path can't be found.</exception>
        protected static string GetReferencePath(in string reference)
        {
            const string guidPrefix = "GUID:";

            if (reference.StartsWith(guidPrefix))
            {
                return AssetDatabase.GUIDToAssetPath(reference[guidPrefix.Length..]);
            }

            foreach (string guid in AssetDatabase.FindAssets($"t:asmdef {reference}"))
            {
                string candidate = AssetDatabase.GUIDToAssetPath(guid);

                if (Path.GetFileNameWithoutExtension(candidate) == candidate)
                {
                    return candidate;
                }
            }

            throw new ArgumentOutOfRangeException(nameof(reference), $"Couldn't find the asset path for \"{reference}\". It will not be validated, try referencing it by GUID instead.");
        }

        /// <summary>
        /// Helper method to check if any item in a list of <see cref="Regex"/> matches the given <paramref name="assemblyDefinitionPath"/>.
        /// </summary>
        protected static bool HasAnyMatch(IReadOnlyList<Regex> regexes, in string assemblyDefinitionPath)
        {
            int count = regexes.Count;

            for (int i = 0; i < count; i++)
            {
                if (regexes[i].IsMatch(assemblyDefinitionPath))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Helper method to validate a list of path patterns, ensuring that they end either with "*" or the given <paramref name="extension"/>.
        /// </summary>
        protected static void ValidatePathPatterns(IList<string> patterns, string extension = ".asmdef")
        {
            int count = patterns.Count;

            for (int i = 0; i < count; i++)
            {
                if (!patterns[i].Contains('/') && !patterns[i].StartsWith("*"))
                {
                    patterns[i] = "*/" + patterns[i];
                }

                if (!patterns[i].EndsWith("*") && !patterns[i].EndsWith(extension))
                {
                    patterns[i] += extension;
                }
            }
        }

        /// <summary>
        /// Unity callback.
        /// </summary>
        protected virtual void Reset()
        {
            _displayError = true;
        }

        /// <summary>
        /// Unity callback.
        /// </summary>
        protected virtual void OnValidate()
        {
            _hasCaches = false;
            _displayError = _includedPatterns.Length == 0 && _excludedPatterns.Length == 0;
            ValidatePathPatterns(_includedPatterns);
            ValidatePathPatterns(_excludedPatterns);
        }

        private void InitializeCaches()
        {
            if (_hasCaches && _excludedRegexes != null && _includedRegexes != null)
            {
                return;
            }

            _hasCaches = true;
            _excludedRegexes = BuildRegexesCache(_excludedPatterns, true);
            _includedRegexes = BuildRegexesCache(_includedPatterns, true);
        }
    }
}
