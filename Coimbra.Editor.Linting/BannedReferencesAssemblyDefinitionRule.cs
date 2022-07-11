using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Coimbra.Editor.Linting
{
    /// <summary>
    /// Define references that will be excluded automatically.
    /// </summary>
    [CreateAssetMenu(menuName = CoimbraUtility.GeneralMenuPath + DefaultAssetMenuPath + "Banned References")]
    public sealed class BannedReferencesAssemblyDefinitionRule : AssemblyDefinitionRuleBase
    {
        [SerializeField]
        [Tooltip("The list of assembly definition path patterns to ban. Any '*' will be used as wildcards.")]
        private string[] _bannedReferences;

        private bool _hasCache;

        private Regex[] _bannedReferencesRegexes;

        /// <summary>
        /// Gets or sets the list of <see cref="AssemblyDefinition"/> path to ban. Any '*' will be used as wildcards.
        /// </summary>
        public IReadOnlyList<string> BannedReferences
        {
            get => _bannedReferences;
            set => _bannedReferences = value.ToArray();
        }

        /// <inheritdoc/>
        public override bool Apply(AssemblyDefinition assemblyDefinition, Object context)
        {
            InitializeCache();

            using (ListPool.Pop(out List<string> list))
            {
                list.EnsureCapacity(assemblyDefinition.References.Length);

                foreach (string reference in assemblyDefinition.References)
                {
                    try
                    {
                        string referencePath = GetReferencePath(reference);

                        if (HasAnyMatch(_bannedReferencesRegexes, referencePath))
                        {
                            Debug.LogWarning($"{assemblyDefinition.Name} had banned reference {referencePath}!", context);
                        }
                        else
                        {
                            list.Add(reference);
                        }
                    }
                    catch
                    {
                        // if the reference can't be found, assume it is not banned
                        list.Add(reference);
                    }
                }

                if (assemblyDefinition.References.Length == list.Count)
                {
                    return false;
                }

                assemblyDefinition.References = list.ToArray();

                return true;
            }
        }

        /// <inheritdoc/>
        protected override void OnValidate()
        {
            base.OnValidate();

            _hasCache = false;
            ValidatePathPatterns(_bannedReferences);
        }

        private void InitializeCache()
        {
            if (_hasCache && _bannedReferencesRegexes != null)
            {
                return;
            }

            _hasCache = true;
            _bannedReferencesRegexes = BuildRegexesCache(_bannedReferences, true);
        }
    }
}
