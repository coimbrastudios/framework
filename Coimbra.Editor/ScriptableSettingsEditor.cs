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
        protected static readonly HashSet<string> DefaultIgnoredProperties = new()
        {
            "m_Script",
            "_type",
        };

        /// <summary>
        /// Excluded fields when <see cref="Type"/> is editor-only.
        /// </summary>
        protected static readonly HashSet<string> DefaultIgnoredPropertiesForEditorOnly = new()
        {
            "m_Script",
            "_preload",
            "_type",
        };

        private const float WindowLabelWidthPercent = 0.3f;

        /// <inheritdoc cref="ScriptableSettings.Type"/>
        protected ScriptableSettingsType Type { get; set; }

        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            using (new LabelWidthScope(EditorGUIUtility.currentViewWidth * WindowLabelWidthPercent, LabelWidthScope.MagnitudeMode.Absolute))
            {
                DrawDefaultInspectorWithSearchSupport(Type.IsEditorOnly() ? DefaultIgnoredPropertiesForEditorOnly : DefaultIgnoredProperties);
            }
        }

        /// <summary>
        /// Override this to define a custom searching logic.
        /// </summary>
        public virtual bool HasSearchInterest(string searchContext)
        {
            return HasSearchInterestInAnyProperty(searchContext, Type.IsEditorOnly() ? DefaultIgnoredPropertiesForEditorOnly : DefaultIgnoredProperties);
        }

        /// <summary>
        /// Helper method that uses <see cref="CoimbraGUIUtility.TryMatchSearch"/> and <see cref="ScriptableSettingsProvider.CurrentSearchContext"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool TryMatchSearch(string targetContent)
        {
            return CoimbraGUIUtility.TryMatchSearch(ScriptableSettingsProvider.CurrentSearchContext, targetContent);
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
            return TryMatchSearch(property) ? EditorGUILayout.PropertyField(property) : null;
        }

        /// <inheritdoc cref="TryPropertyField(SerializedProperty)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool? TryPropertyField(SerializedProperty property, bool includeChildren)
        {
            return TryMatchSearch(property) ? EditorGUILayout.PropertyField(property, includeChildren) : null;
        }

        /// <inheritdoc cref="TryPropertyField(SerializedProperty)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool? TryPropertyField(SerializedProperty property, GUIContent label)
        {
            return TryMatchSearch(property) ? EditorGUILayout.PropertyField(property, label) : null;
        }

        /// <inheritdoc cref="TryPropertyField(SerializedProperty)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool? TryPropertyField(SerializedProperty property, GUIContent label, bool includeChildren)
        {
            return TryMatchSearch(property) ? EditorGUILayout.PropertyField(property, label, includeChildren) : null;
        }

        /// <summary>
        /// Unity callback.
        /// </summary>
        protected virtual void OnEnable()
        {
            Type = ScriptableSettings.GetType(target.GetType());
        }

        /// <summary>
        /// Draws the default inspector with basic search functionality for either the Preferences window or the Project Settings window.
        /// </summary>
        protected void DrawDefaultInspectorWithSearchSupport(HashSet<string> ignoredProperties)
        {
            serializedObject.UpdateIfRequiredOrScript();

            using EditorGUI.ChangeCheckScope changeCheckScope = new();
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (ignoredProperties.Contains(iterator.name))
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

            if (!changeCheckScope.changed)
            {
                return;
            }

            serializedObject.ApplyModifiedProperties();

            foreach (Object o in targets)
            {
                if (o is ScriptableSettings settings)
                {
                    EditorUtility.ClearDirty(o);
                    settings.Save();
                }
            }
        }

        /// <summary>
        /// Checks each property for a match with <paramref name="searchContext"/>.
        /// </summary>
        protected bool HasSearchInterestInAnyProperty(in string searchContext, HashSet<string> ignoredProperties)
        {
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (ignoredProperties.Contains(iterator.name))
                {
                    continue;
                }

                if (CoimbraGUIUtility.TryMatchSearch(searchContext, iterator.displayName))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
