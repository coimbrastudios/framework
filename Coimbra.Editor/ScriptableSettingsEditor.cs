using UnityEditor;

namespace Coimbra.Editor
{
    [CustomEditor(typeof(ScriptableSettings), true, isFallback = true)]
    public class ScriptableSettingsEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Default excluded fields.
        /// </summary>
        protected static readonly string[] DefaultExcludedFields = new[]
        {
            "m_Script",
            "_type",
        };

        /// <summary>
        /// Excluded fields when <see cref="Type"/> is editor-only.
        /// </summary>
        protected static readonly string[] ExcludedFieldsForEditorOnly = new[]
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
            DrawPropertiesExcluding(serializedObject, Type.IsEditorOnly() ? ExcludedFieldsForEditorOnly : DefaultExcludedFields);
            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnEnable()
        {
            Type = ScriptableSettings.GetType(target.GetType());
        }
    }
}
