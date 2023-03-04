using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Scope for managing the <see cref="EditorGUIUtility.hierarchyMode"/>.
    /// </summary>
    public sealed class HierarchyModeScope : GUI.Scope
    {
        /// <summary>
        /// The value before entering this scope.
        /// </summary>
        public readonly bool SavedHierarchyMode;

        public HierarchyModeScope()
        {
            SavedHierarchyMode = EditorGUIUtility.hierarchyMode;
        }

        public HierarchyModeScope(bool hierarchyMode)
        {
            SavedHierarchyMode = EditorGUIUtility.hierarchyMode;
            EditorGUIUtility.hierarchyMode = hierarchyMode;
        }

        protected override void CloseScope()
        {
            EditorGUIUtility.hierarchyMode = SavedHierarchyMode;
        }
    }
}
