using System.Collections.Generic;
using UnityEditor;

namespace Coimbra.Editor
{
    /// <summary>
    /// Editor for <see cref="ScriptableSettings"/>.
    /// </summary>
    [CustomEditor(typeof(ScriptableSettings), true, isFallback = true)]
    public class ScriptableSettingsEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Default excluded fields.
        /// </summary>
        protected static readonly HashSet<string> DefaultExcludedFields = new HashSet<string>
        {
            "m_Script",
            "_type",
        };

        /// <summary>
        /// Excluded fields when <see cref="Type"/> is editor-only.
        /// </summary>
        protected static readonly HashSet<string> ExcludedFieldsForEditorOnly = new HashSet<string>
        {
            "m_Script",
            "_preload",
            "_type",
        };

        /// <inheritdoc cref="ScriptableSettings.Type"/>
        protected ScriptableSettingsType Type { get; set; }

        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            HashSet<string> propertiesToExclude = Type.IsEditorOnly() ? ExcludedFieldsForEditorOnly : DefaultExcludedFields;
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (propertiesToExclude.Contains(iterator.name))
                {
                    continue;
                }

                if (ScriptableSettingsProvider.CurrentSearchContext == null || CoimbraEditorGUIUtility.TryMatchSearch(ScriptableSettingsProvider.CurrentSearchContext, iterator.displayName))
                {
                    EditorGUILayout.PropertyField(iterator, true);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        public virtual bool HasSearchInterest(string searchContext)
        {
            serializedObject.UpdateIfRequiredOrScript();

            HashSet<string> propertiesToExclude = Type.IsEditorOnly() ? ExcludedFieldsForEditorOnly : DefaultExcludedFields;
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (propertiesToExclude.Contains(iterator.name))
                {
                    continue;
                }

                if (ScriptableSettingsProvider.CurrentSearchContext == null || CoimbraEditorGUIUtility.TryMatchSearch(ScriptableSettingsProvider.CurrentSearchContext, iterator.displayName))
                {
                    return true;
                }
            }

            serializedObject.ApplyModifiedProperties();

            return false;
        }

        protected virtual void OnEnable()
        {
            Type = ScriptableSettings.GetType(target.GetType());
        }
    }
}
