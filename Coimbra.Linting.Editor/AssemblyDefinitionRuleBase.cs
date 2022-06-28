using Coimbra.Editor;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Coimbra.Linting.Editor
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
        [Tooltip("The list of targets to apply this rule. It accepts any path relative to the project folder. If empty it will include everything by default.")]
        private string[] _includedMask = Array.Empty<string>();

        [SerializeField]
        [Tooltip("The list of exceptions in the Included Mask to not apply this rule. It accepts any path relative to the project folder. If empty it will not exclude anything by default.")]
        private string[] _excludedMask = Array.Empty<string>();

        [SerializeField]
        [UsedImplicitly]
        [HideInInspector]
        private bool _displayError;

        private bool _hasCaches;

        private Regex[] _excludedRegexes;

        private Regex[] _includedRegexes;

        /// <summary>
        /// Gets or sets the list of targets to apply this rule. It accepts any path relative to the project folder. If empty it will include everything by default.
        /// </summary>
        [NotNull]
        public IReadOnlyList<string> ExcludedMask
        {
            get => _excludedMask;
            set
            {
                _excludedMask = value.ToArray();
                OnValidate();
            }
        }

        /// <summary>
        /// Gets or sets the list of exceptions in the Included Mask to not apply this rule. It accepts any path relative to the project folder. If empty it will not exclude anything by default.
        /// </summary>
        [NotNull]
        public IReadOnlyList<string> IncludedMask
        {
            get => _includedMask;
            set
            {
                _includedMask = value.ToArray();
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

        internal static bool HasAnyMatch(IReadOnlyList<Regex> regexes, in string assemblyDefinitionPath)
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
            _displayError = _includedMask.Length == 0 && _excludedMask.Length == 0;

            const string asmdef = "asmdef";

            for (int i = 0; i < _includedMask.Length; i++)
            {
                if (!_includedMask[i].EndsWith("*") && !_includedMask[i].EndsWith($".{asmdef}"))
                {
                    _includedMask[i] += $".{asmdef}";
                }
            }

            for (int i = 0; i < _excludedMask.Length; i++)
            {
                if (!_excludedMask[i].EndsWith("*") && !_excludedMask[i].EndsWith($".{asmdef}"))
                {
                    _excludedMask[i] += $".{asmdef}";
                }
            }
        }

        private void InitializeCaches()
        {
            if (_hasCaches && _excludedRegexes != null && _includedRegexes != null)
            {
                return;
            }

            _excludedRegexes = new Regex[_excludedMask.Length];

            for (int i = 0; i < _excludedMask.Length; i++)
            {
                _excludedRegexes[i] = PathUtility.GetRegexFromPattern(_excludedMask[i], true);
            }

            _includedRegexes = new Regex[_includedMask.Length];

            for (int i = 0; i < _includedMask.Length; i++)
            {
                _includedRegexes[i] = PathUtility.GetRegexFromPattern(_includedMask[i], true);
            }

            _hasCaches = true;
        }
    }
}
