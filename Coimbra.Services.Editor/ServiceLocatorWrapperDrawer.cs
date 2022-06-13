using Coimbra.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Coimbra.Services.Editor
{
    [CustomPropertyDrawer(typeof(ServiceLocatorWrapper))]
    internal sealed class ServiceLocatorWrapperDrawer : PropertyDrawer
    {
        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ReorderableList list = property.FindPropertyRelative(nameof(ServiceLocatorWrapper.Services)).ToReorderableList(InitializeReorderableList);

            return list.serializedProperty.isExpanded ? EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + list.GetHeight() : EditorGUIUtility.singleLineHeight;
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty id = property.FindPropertyRelative(nameof(ServiceLocatorWrapper.Id));
            SerializedProperty allowFallbackToShared = property.FindPropertyRelative(nameof(ServiceLocatorWrapper.AllowFallbackToShared));
            ReorderableList list = property.FindPropertyRelative(nameof(ServiceLocatorWrapper.Services)).ToReorderableList(InitializeReorderableList);
            position.height = EditorGUIUtility.singleLineHeight;

            list.serializedProperty.isExpanded = EditorGUI.Foldout(position, list.serializedProperty.isExpanded, id.stringValue, true);

            Rect togglePosition = position;
            togglePosition.xMin = togglePosition.xMax - 170;
            EditorGUI.ToggleLeft(togglePosition, allowFallbackToShared.displayName, allowFallbackToShared.boolValue);

            if (!list.serializedProperty.isExpanded)
            {
                return;
            }

            using (new EditorGUI.IndentLevelScope(1))
            {
                position.y += position.height;
                position.height = list.GetHeight();
                list.DoList(EditorGUI.IndentedRect(position));
            }
        }

        private static void InitializeReorderableList(ReorderableList list)
        {
            static void drawNone(Rect rect)
            {
                EditorGUI.LabelField(rect, "Locator is Empty");
            }

            list.draggable = false;
            list.displayAdd = false;
            list.displayRemove = false;
            list.footerHeight = 0;
            list.headerHeight = 0;
            list.drawNoneElementCallback = drawNone;

            list.drawElementCallback = delegate(Rect rect, int index, bool active, bool focused)
            {
                EditorGUI.PropertyField(rect, list.serializedProperty.GetArrayElementAtIndex(index));
            };

            list.elementHeightCallback = delegate(int index)
            {
                return list.serializedProperty.arraySize == 0 ? 0 : EditorGUI.GetPropertyHeight(list.serializedProperty.GetArrayElementAtIndex(index)) + EditorGUIUtility.standardVerticalSpacing;
            };
        }
    }
}
