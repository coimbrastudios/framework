using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Scope for managing the <see cref="EditorGUI.showMixedValue"/>.
    /// </summary>
    public sealed class ShowMixedValueScope : GUI.Scope
    {
        /// <summary>
        /// The value before entering this scope.
        /// </summary>
        public readonly bool SavedShowMixedValue;

        public ShowMixedValueScope()
        {
            SavedShowMixedValue = EditorGUI.showMixedValue;
        }

        public ShowMixedValueScope(bool showMixedValue)
        {
            SavedShowMixedValue = EditorGUI.showMixedValue;
            EditorGUI.showMixedValue = showMixedValue;
        }

        protected override void CloseScope()
        {
            EditorGUI.showMixedValue = SavedShowMixedValue;
        }
    }
}
