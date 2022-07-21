using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor.Linting
{
    [CustomEditor(typeof(LintingSettings))]
    internal sealed class LintingSettingsEditor : ScriptableSettingsEditor
    {
        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Run"))
            {
                LintingSettings.InitializeAssemblyDefinitionRules();
            }

            base.OnInspectorGUI();
        }
    }
}
