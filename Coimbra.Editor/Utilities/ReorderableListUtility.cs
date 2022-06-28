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
        /// <inheritdoc cref="SerializedProperty.serializedObject"/>
        public static SerializedObject GetSerializedObject(this ReorderableList reorderableList)
        {
            return reorderableList.serializedProperty.serializedObject;
        }

        /// <inheritdoc cref="SerializedObject.targetObject"/>
        public static Object GetTargetObject(this ReorderableList reorderableList)
        {
            return reorderableList.serializedProperty.serializedObject.targetObject;
        }

        /// <inheritdoc cref="SerializedObject.targetObjects"/>
        public static Object[] GetTargetObjects(this ReorderableList reorderableList)
        {
            return reorderableList.serializedProperty.serializedObject.targetObjects;
        }
    }
}
