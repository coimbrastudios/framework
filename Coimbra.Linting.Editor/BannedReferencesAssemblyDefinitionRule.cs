using Coimbra.Editor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Linting.Editor
{
    /// <summary>
    /// Define references that will be excluded automatically.
    /// </summary>
    [CreateAssetMenu(menuName = CoimbraUtility.GeneralMenuPath + DefaultAssetMenuPath + "Banned References")]
    public sealed class BannedReferencesAssemblyDefinitionRule : AssemblyDefinitionRuleBase
    {
        private const string GuidPrefix = "GUID:";

        [SerializeField]
        [Tooltip("The list os assembly definition names to ban. Any '*' will be used as wildcards.")]
        private string[] _bannedReferences;

        private bool _hasCache;

        private Regex[] _bannedReferencesRegexes;

        /// <summary>
        /// The list of <see cref="AssemblyDefinition"/> names to ban. Any '*' will be used as wildcards.
        /// </summary>
        public IReadOnlyList<string> BannedReferences
        {
            get => _bannedReferences;
            set => _bannedReferences = value.ToArray();
        }

        /// <inheritdoc/>
        public override bool Apply(AssemblyDefinition assemblyDefinition)
        {
            InitializeCache();

            using (ListPool.Pop(out List<string> list))
            {
                list.EnsureCapacity(assemblyDefinition.References.Length);

                foreach (string reference in assemblyDefinition.References)
                {
                    string referenceName = reference.StartsWith(GuidPrefix) ? Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(reference.Substring(GuidPrefix.Length))) : reference;

                    if (IsBannedReference(referenceName))
                    {
                        Debug.LogWarning($"{assemblyDefinition.Name} had banned reference to {referenceName}!");

                        continue;
                    }

                    list.Add(reference);
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
        }

        private void InitializeCache()
        {
            if (_hasCache && _bannedReferencesRegexes != null)
            {
                return;
            }

            _bannedReferencesRegexes = new Regex[_bannedReferences.Length];

            for (int i = 0; i < _bannedReferences.Length; i++)
            {
                _bannedReferencesRegexes[i] = PathUtility.GetRegexFromPattern(_bannedReferences[i], true);
            }

            _hasCache = true;
        }

        private bool IsBannedReference(in string reference)
        {
            foreach (Regex regex in _bannedReferencesRegexes)
            {
                if (regex.IsMatch(reference))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
