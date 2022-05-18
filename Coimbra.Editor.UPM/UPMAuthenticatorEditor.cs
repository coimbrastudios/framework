using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor.UPM
{
    [CustomEditor(typeof(UPMAuthenticator))]
    internal sealed class UPMAuthenticatorEditor : UnityEditor.Editor
    {
        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Update"))
            {
                UPMAuthenticator.Update();
            }
        }
    }
}
