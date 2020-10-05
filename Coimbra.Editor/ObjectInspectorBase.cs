using UnityEditor;
using UnityEngine;

namespace Coimbra
{
    [CanEditMultipleObjects]
    public abstract class ObjectInspectorBase : Editor
    {
        public static bool IsEnabled = true;

        public override void OnInspectorGUI()
        {
            if (IsEnabled)
            {
                EditorGUILayout.LabelField("fun");
                DrawExtendedInspector();
            }
            else
            {
                DrawDefaultInspector();
            }
        }

        protected void DrawExtendedInspector()
        {
            serializedObject.Update();

            SerializedProperty iterator = serializedObject.GetIterator();

            if (iterator.NextVisible(true))
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.PropertyField(iterator, true);
                }

                while (iterator.NextVisible(false))
                {
                    EditorGUILayout.PropertyField(iterator, true);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
