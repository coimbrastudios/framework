using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Editor for <see cref="ScriptableSettings"/>.
    /// </summary>
    [CanEditMultipleObjects]
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

                if (Type == ScriptableSettingsType.Custom)
                {
                    EditorGUILayout.PropertyField(iterator, true);
                }
                else
                {
                    TryPropertyField(iterator, true);
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

                if (CoimbraEditorGUIUtility.TryMatchSearch(searchContext, iterator.displayName))
                {
                    return true;
                }
            }

            serializedObject.ApplyModifiedProperties();

            return false;
        }

        /// <summary>
        /// Helper method that uses <see cref="CoimbraEditorGUIUtility.TryMatchSearch"/> and <see cref="ScriptableSettingsProvider.CurrentSearchContext"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool TryMatchSearch(string targetContent)
        {
            return CoimbraEditorGUIUtility.TryMatchSearch(ScriptableSettingsProvider.CurrentSearchContext, targetContent);
        }

        /// <inheritdoc cref="TryMatchSearch(string)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool TryMatchSearch(SerializedProperty property)
        {
            return ScriptableSettingsProvider.CurrentSearchContext == null || TryMatchSearch(property.displayName);
        }

        /// <summary>
        /// Helper method that uses <see cref="TryMatchSearch(string)"/> before drawing a property field.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool? TryPropertyField(SerializedProperty property)
        {
            return TryMatchSearch(property) ? (bool?)EditorGUILayout.PropertyField(property) : null;
        }

        /// <inheritdoc cref="TryPropertyField(SerializedProperty)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool? TryPropertyField(SerializedProperty property, bool includeChildren)
        {
            return TryMatchSearch(property) ? (bool?)EditorGUILayout.PropertyField(property, includeChildren) : null;
        }

        /// <inheritdoc cref="TryPropertyField(SerializedProperty)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool? TryPropertyField(SerializedProperty property, GUIContent label)
        {
            return TryMatchSearch(property) ? (bool?)EditorGUILayout.PropertyField(property, label) : null;
        }

        /// <inheritdoc cref="TryPropertyField(SerializedProperty)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool? TryPropertyField(SerializedProperty property, GUIContent label, bool includeChildren)
        {
            return TryMatchSearch(property) ? (bool?)EditorGUILayout.PropertyField(property, label, includeChildren) : null;
        }

        /// <summary>
        /// Unity callback.
        /// </summary>
        protected virtual void OnEnable()
        {
            Type = ScriptableSettings.GetType(target.GetType());
        }
    }
}
