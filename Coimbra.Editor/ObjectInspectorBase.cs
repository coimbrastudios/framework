using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Coimbra
{
    [CanEditMultipleObjects]
    public abstract class ObjectInspectorBase : Editor
    {
        protected bool IsExtendedInspectorEnabled => LocalSettingsProvider.EnableExtendedInspectorGlobally || serializedObject.targetObject.GetType().GetCustomAttribute<ExtendedInspectorAttribute>() != null;

        public override void OnInspectorGUI()
        {
            if (IsExtendedInspectorEnabled)
            {
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
