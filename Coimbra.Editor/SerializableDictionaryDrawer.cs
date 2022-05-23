using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Drawer for <see cref="SerializableDictionary{TKey,TValue}"/>.
    /// </summary>
    [InitializeOnLoad]
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>), true)]
    public class SerializableDictionaryDrawer : PropertyDrawer
    {
        protected const float CountFieldSize = 50;

        protected const float LabelViewWidthPercent = 0.4f;

        protected const float NestedLabelViewWidthPercent = 0.3f;

        protected const string ItemsProperty = "_items";

        protected const string KeyProperty = "Key";

        protected const string NewEntryProperty = "_newEntry";

        protected const string ValueProperty = "Value";

        private const string ModifyDisabledMessage = "Can't resize while editing multiple objects!";

        static SerializableDictionaryDrawer()
        {
            Undo.postprocessModifications -= HandlePostprocessModifications;
            Undo.postprocessModifications += HandlePostprocessModifications;
        }

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;

            if (!property.isExpanded)
            {
                return height;
            }

            ReorderableList list = property.FindPropertyRelative(ItemsProperty).ToReorderableList(InitializeReorderableList);
            bool enabled = GUI.enabled;
            list.displayAdd = enabled;
            list.displayRemove = enabled;
            list.footerHeight = enabled ? EditorGUIUtility.singleLineHeight : 0;
            height += EditorGUIUtility.standardVerticalSpacing + list.GetHeight();

            SerializedProperty newEntryProperty = property.FindPropertyRelative(NewEntryProperty);

            return newEntryProperty.isExpanded ? height + GetItemHeight(newEntryProperty) + EditorGUIUtility.standardVerticalSpacing : height;
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool isEditingMultipleObjects = property.serializedObject.isEditingMultipleObjects;
            Rect headerPosition = position;
            headerPosition.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(headerPosition, property, label, false);

            SerializedProperty itemsProperty = property.FindPropertyRelative(ItemsProperty);

            using (new EditorGUI.DisabledScope(true))
            {
                headerPosition.xMin += headerPosition.width - CountFieldSize;

                static bool shouldShowMixedValue(SerializedProperty property)
                {
                    using (ListPool.Pop(out List<ICollection> collections))
                    {
                        property.GetValues(collections);

                        int size = property.arraySize;

                        foreach (ICollection collection in collections)
                        {
                            if (collection.Count != size)
                            {
                                return true;
                            }
                        }

                        return false;
                    }
                }

                using (new ShowMixedValueScope(isEditingMultipleObjects && shouldShowMixedValue(itemsProperty)))
                {
                    EditorGUI.IntField(headerPosition, itemsProperty.arraySize);
                }
            }

            if (!property.isExpanded)
            {
                return;
            }

            ReorderableList list = itemsProperty.ToReorderableList(InitializeReorderableList);
            Rect listPosition = position;
            bool enabled = GUI.enabled;
            list.displayAdd = enabled;
            list.displayRemove = enabled;
            list.footerHeight = enabled ? EditorGUIUtility.singleLineHeight : 0;
            listPosition.yMin += headerPosition.height + EditorGUIUtility.standardVerticalSpacing;
            listPosition.height = list.GetHeight();

            using (new LabelWidthScope(position.width * LabelViewWidthPercent, LabelWidthScope.MagnitudeMode.Absolute))
            {
                list.DoList(listPosition);
            }

            position.y += headerPosition.height + EditorGUIUtility.standardVerticalSpacing + listPosition.height - EditorGUIUtility.singleLineHeight;
            position.height = EditorGUIUtility.singleLineHeight;

            if (CanModifyList(list))
            {
                SerializedProperty newEntryProperty = property.FindPropertyRelative(NewEntryProperty);
                position.xMin += ReorderableList.Defaults.dragHandleWidth;

                if (!EditorGUI.PropertyField(position, newEntryProperty, false))
                {
                    return;
                }

                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                position.height = GetItemHeight(newEntryProperty);

                using (new LabelWidthScope((position.width + ReorderableList.Defaults.dragHandleWidth) * LabelViewWidthPercent, LabelWidthScope.MagnitudeMode.Absolute))
                {
                    DrawItem(position, newEntryProperty, false);
                }
            }
            else if (GUI.enabled)
            {
                EditorGUI.LabelField(position, ModifyDisabledMessage);
            }
        }

        private static bool CanModifyList(ReorderableList list)
        {
            return !list.serializedProperty.serializedObject.isEditingMultipleObjects && GUI.enabled;
        }

        private static void DrawItem(Rect position, SerializedProperty property, bool disableKeyField)
        {
            static void draw(Rect position, SerializedProperty property)
            {
                using (new LabelWidthScope(EditorGUIUtility.labelWidth * NestedLabelViewWidthPercent, LabelWidthScope.MagnitudeMode.Absolute))
                {
                    if (property.isArray && property.propertyType != SerializedPropertyType.String)
                    {
                        EditorGUI.PropertyField(position, property);
                    }
                    else if (property.GetValue() is ISerializableCollection)
                    {
                        EditorGUI.PropertyField(position, property);
                    }
                    else
                    {
                        EditorGUI.PropertyField(position, property, true);
                    }
                }
            }

            SerializedProperty keyProperty = property.FindPropertyRelative(KeyProperty);
            SerializedProperty valueProperty = property.FindPropertyRelative(ValueProperty);
            Rect keyPosition = position.WithHeight(EditorGUI.GetPropertyHeight(keyProperty, GUIContent.none));
            Rect valuePosition = position.WithHeight(EditorGUI.GetPropertyHeight(valueProperty, GUIContent.none));
            keyPosition.width = EditorGUIUtility.labelWidth - EditorStyles.foldout.CalcSize(GUIContent.none).x - EditorGUIUtility.standardVerticalSpacing;
            valuePosition.xMin += EditorGUIUtility.labelWidth;

            using (new EditorGUI.DisabledScope(disableKeyField))
            {
                draw(keyPosition, keyProperty);
            }

            draw(valuePosition, valueProperty);
        }

        private static float GetItemHeight(SerializedProperty property)
        {
            float keyPropertyHeight = EditorGUI.GetPropertyHeight(property.FindPropertyRelative(KeyProperty), GUIContent.none);
            float valuePropertyHeight = EditorGUI.GetPropertyHeight(property.FindPropertyRelative(ValueProperty), GUIContent.none);

            return Mathf.Max(keyPropertyHeight, valuePropertyHeight);
        }

        private static UndoPropertyModification[] HandlePostprocessModifications(UndoPropertyModification[] modifications)
        {
            for (int i = 0; i < modifications.Length; i++)
            {
                ref UndoPropertyModification modification = ref modifications[i];

                if (modification.currentValue.IsReorderableList(out ReorderableList list) && list.serializedProperty.GetScope()!.GetValue(list.GetTargetObject()) is ISerializableDictionary serializableDictionary)
                {
                    serializableDictionary.ProcessUndo();
                }
            }

            return modifications;
        }

        private static void InitializeReorderableList(ReorderableList list)
        {
            static void add(ReorderableList list)
            {
                Undo.RecordObject(list.serializedProperty.serializedObject.targetObject, "Add Element To Dictionary");
                list.serializedProperty.GetScope()!.TryGetValue(list.GetTargetObject(), out ISerializableDictionary serializableDictionary);
                serializableDictionary!.ProcessAdd();
                list.serializedProperty.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                list.serializedProperty.serializedObject.UpdateIfRequiredOrScript();
                EditorUtility.SetDirty(list.serializedProperty.serializedObject.targetObject);
            }

            static bool canAdd(ReorderableList list)
            {
                return CanModifyList(list) && list.serializedProperty.GetScope()!.GetValue(list.GetTargetObject()) is ISerializableDictionary { IsNewEntryValid: true };
            }

            list.headerHeight = 0;
#if UNITY_2021_3_OR_NEWER
            list.multiSelect = true;
#endif
            list.onAddCallback = add;
            list.onCanAddCallback = canAdd;
            list.onCanRemoveCallback = CanModifyList;

            list.drawNoneElementCallback = delegate(Rect position)
            {
                EditorGUI.LabelField(position, "Dictionary is Empty");
            };

            list.drawElementCallback = delegate(Rect position, int index, bool active, bool focused)
            {
                Rect drawPosition = position;
                drawPosition.yMin += EditorGUIUtility.standardVerticalSpacing;

                SerializedProperty elementProperty = list.serializedProperty.GetArrayElementAtIndex(index);
                DrawItem(drawPosition, elementProperty, true);
            };

            list.elementHeightCallback = delegate(int index)
            {
                if (list.serializedProperty.arraySize == 0)
                {
                    return 0;
                }

                SerializedProperty elementProperty = list.serializedProperty.GetArrayElementAtIndex(index);

                return GetItemHeight(elementProperty) + EditorGUIUtility.standardVerticalSpacing;
            };
        }
    }
}
