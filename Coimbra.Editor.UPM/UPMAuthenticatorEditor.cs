using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor.UPM
{
    [CustomEditor(typeof(UPMAuthenticator))]
    internal sealed class UPMAuthenticatorEditor : ScriptableSettingsEditor
    {
        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Update"))
            {
                UPMAuthenticator.Update();
            }

            base.OnInspectorGUI();
        }
    }
}
