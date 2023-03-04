using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Editor for <see cref="ScriptableSettings"/>.
    /// </summary>
    /// <remarks>
    /// Inherit from this class to preserve search support inside Preferences or Project Settings window when creating a custom <see cref="UnityEditor.Editor"/> for any <see cref="ScriptableSettings"/>.
    /// The functionalities provided here are mostly ignored if <see cref="ScriptableSettings"/> is <see cref="ScriptableSettingsType.Custom"/>.
    /// <para></para>
    /// The <see cref="HasSearchInterest"/> will be used to check if the object itself should be visible in its respective window.
    /// The default implementation uses <see cref="HasSearchInterestInAnyProperty"/> alongside <see cref="CustomIgnoredProperties"/> or <see cref="DefaultIgnoredProperties"/> (according the <see cref="Type"/>).
    /// <para></para>
    /// To draw a field with search support you need to use either <see cref="TryMatchSearch(string)"/> or <see cref="TryMatchSearch(SerializedProperty)"/> to check if the field should actually be drawn.
    /// There is also <see cref="TryPropertyField(UnityEditor.SerializedProperty)"/> (and some overloads) provided for simple cases.
    /// <para></para>
    /// You can also draw the default inspector using <see cref="DrawDefaultInspectorWithSearchSupport"/>.
    /// </remarks>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ScriptableSettings), true, isFallback = true)]
    public class ScriptableSettingsEditor : UnityEditor.Editor
    {
        internal const float WindowLabelWidthPercent = 0.3f;

        /// <summary>
        /// Default excluded fields for <see cref="ScriptableSettingsType.Custom"/> instances.
        /// </summary>
        protected static readonly HashSet<string> CustomIgnoredProperties = new()
        {
            "m_Script",
            "_type",
        };

        /// <summary>
        /// Excluded fields when <see cref="Type"/> is not <see cref="ScriptableSettingsType.Custom"/>.
        /// </summary>
        protected static readonly HashSet<string> DefaultIgnoredProperties = new()
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
            using (new LabelWidthScope(EditorGUIUtility.currentViewWidth * WindowLabelWidthPercent, LabelWidthScope.MagnitudeMode.Absolute))
            {
                DrawDefaultInspectorWithSearchSupport(Type == ScriptableSettingsType.Custom ? CustomIgnoredProperties : DefaultIgnoredProperties);
            }
        }

        /// <summary>
        /// Override this to define if the <see cref="ScriptableSettings"/> is currently relevant for a search.
        /// </summary>
        public virtual bool HasSearchInterest(string searchContext)
        {
            return HasSearchInterestInAnyProperty(searchContext, Type == ScriptableSettingsType.Custom ? CustomIgnoredProperties : DefaultIgnoredProperties);
        }

        /// <summary>
        /// Helper method that uses <see cref="EngineUtility.TryMatchSearch"/> and <see cref="ScriptableSettingsSearchScope.CurrentSearch"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool TryMatchSearch(string targetContent)
        {
            return EngineUtility.TryMatchSearch(ScriptableSettingsSearchScope.CurrentSearch, targetContent);
        }

        /// <inheritdoc cref="TryMatchSearch(string)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool TryMatchSearch(SerializedProperty property)
        {
            return ScriptableSettingsSearchScope.CurrentSearch == null || TryMatchSearch(property.displayName);
        }

        /// <summary>
        /// Helper method that uses <see cref="TryMatchSearch(string)"/> before drawing a property field.
        /// </summary>
        /// <returns>Null if the field doesn't match the current search, otherwise it will return the same as <see cref="EditorGUILayout.PropertyField(UnityEditor.SerializedProperty,UnityEngine.GUILayoutOption[])"/>.</returns>
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
            Type = ScriptableSettings.GetTypeData(target.GetType());
        }

        /// <summary>
        /// Draws the default inspector with basic search functionality for either the Preferences window or the Project Settings window.
        /// </summary>
        protected void DrawDefaultInspectorWithSearchSupport(HashSet<string> ignoredProperties = null)
        {
            serializedObject.UpdateIfRequiredOrScript();

            using EditorGUI.ChangeCheckScope changeCheckScope = new();
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (ignoredProperties?.Contains(iterator.name) ?? false)
                {
                    continue;
                }

                using (new EditorGUI.DisabledScope(false))
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
                    settings.SaveAsset();
                    EditorUtility.ClearDirty(o);
                }
            }
        }

        /// <summary>
        /// Checks each property for a match with <paramref name="searchContext"/>.
        /// </summary>
        protected bool HasSearchInterestInAnyProperty(in string searchContext, HashSet<string> ignoredProperties = null)
        {
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (ignoredProperties?.Contains(iterator.name) ?? false)
                {
                    continue;
                }

                if (EngineUtility.TryMatchSearch(searchContext, iterator.displayName))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
