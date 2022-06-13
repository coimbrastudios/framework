using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Utility methods for <see cref="ReorderableList"/> type.
    /// </summary>
    public static class ReorderableListUtility
    {
        /// <see cref="SerializedProperty.serializedObject"/>
        public static SerializedObject GetSerializedObject(this ReorderableList reorderableList)
        {
            return reorderableList.serializedProperty.serializedObject;
        }

        /// <see cref="SerializedObject.targetObject"/>
        public static Object GetTargetObject(this ReorderableList reorderableList)
        {
            return reorderableList.serializedProperty.serializedObject.targetObject;
        }

        /// <see cref="SerializedObject.targetObjects"/>
        public static Object[] GetTargetObjects(this ReorderableList reorderableList)
        {
            return reorderableList.serializedProperty.serializedObject.targetObjects;
        }
    }
}
