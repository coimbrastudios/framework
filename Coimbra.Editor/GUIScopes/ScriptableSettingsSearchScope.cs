#nullable enable

using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Scope for managing the search context for any <see cref="ScriptableSettingsEditor"/>.
    /// </summary>
    public sealed class ScriptableSettingsSearchScope : GUI.Scope
    {
        /// <summary>
        /// The value before entering this scope.
        /// </summary>
        public readonly string? SavedSearch;

        public ScriptableSettingsSearchScope(string? search)
        {
            SavedSearch = CurrentSearch;
            CurrentSearch = search;
        }

        /// <summary>
        /// Gets the current search context.
        /// </summary>
        public static string? CurrentSearch { get; private set; }

        protected override void CloseScope()
        {
            CurrentSearch = SavedSearch;
        }
    }
}
