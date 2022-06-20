using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
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
    public sealed class SerializableDictionaryDrawer : PropertyDrawer
    {
        private const float CountFieldSize = 50;

        private const float KeyWidthPercent = 0.4f;

        private const float LabelWidthPercent = 0.5f;

        private const string KeyProperty = "Key";

        private const string ListProperty = "_list";

        private const string ModifyDisabledMessage = "Can't add new item while editing multiple objects!";

        private const string NewProperty = "_new";

        private const string ValueProperty = "Value";

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

            ReorderableList list = property.FindPropertyRelative(ListProperty).ToReorderableList(InitializeReorderableList);
            bool displayFooter = GUI.enabled && list.serializedProperty.GetScopeInfo()!.FieldInfo.GetCustomAttribute<DisableResizeAttribute>() == null;
            list.displayAdd = displayFooter;
            list.displayRemove = displayFooter;
            list.footerHeight = displayFooter ? EditorGUIUtility.singleLineHeight : 0;
            height += EditorGUIUtility.standardVerticalSpacing + list.GetHeight();

            if (!displayFooter)
            {
                return height;
            }

            SerializedProperty newProperty = property.FindPropertyRelative(NewProperty);
            height += EditorGUI.GetPropertyHeight(newProperty, newProperty.isExpanded) - EditorGUIUtility.singleLineHeight;

            return height;
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ReorderableList list = property.FindPropertyRelative(ListProperty).ToReorderableList(InitializeReorderableList);
            Rect headerPosition = DrawHeader(position, label, property, list);

            if (!property.isExpanded)
            {
                return;
            }

            Rect listPosition = position;
            bool canResize = GUI.enabled && list.serializedProperty.GetScopeInfo()!.FieldInfo.GetCustomAttribute<DisableResizeAttribute>() == null;
            list.displayAdd = canResize;
            list.displayRemove = canResize;
            list.footerHeight = canResize ? EditorGUIUtility.singleLineHeight : 0;
            listPosition.yMin += headerPosition.height + EditorGUIUtility.standardVerticalSpacing;
            listPosition.height = list.GetHeight();

            using (new LabelWidthScope(position.width * KeyWidthPercent, LabelWidthScope.MagnitudeMode.Absolute))
            {
                list.DoList(listPosition);
            }

            if (!canResize)
            {
                return;
            }

            position.y += headerPosition.height + EditorGUIUtility.standardVerticalSpacing + listPosition.height - EditorGUIUtility.singleLineHeight;

            if (property.serializedObject.isEditingMultipleObjects)
            {
                position.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(position, ModifyDisabledMessage);

                return;
            }

            SerializedProperty newProperty = property.FindPropertyRelative(NewProperty);
            position.x += EditorGUIUtility.standardVerticalSpacing;
            position.width *= KeyWidthPercent;
            position.width += EditorGUIUtility.standardVerticalSpacing;
            position.xMin += ReorderableList.Defaults.dragHandleWidth;
            position.x += +EditorStyles.foldout.CalcSize(GUIContent.none).x;
            position.height = EditorGUI.GetPropertyHeight(newProperty, true);

            using (new LabelWidthScope(position.width * LabelWidthPercent + EditorGUIUtility.standardVerticalSpacing, LabelWidthScope.MagnitudeMode.Absolute))
            {
                EditorGUI.PropertyField(position, newProperty, true);
            }
        }

        private static Rect DrawHeader(Rect position, GUIContent label, SerializedProperty property, ReorderableList list)
        {
            bool isEditingMultipleObjects = property.serializedObject.isEditingMultipleObjects;
            Rect headerPosition = position;
            headerPosition.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(headerPosition, property, label, false);

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

                using (new ShowMixedValueScope(isEditingMultipleObjects && shouldShowMixedValue(list.serializedProperty)))
                {
                    EditorGUI.IntField(headerPosition, list.serializedProperty.arraySize);
                }
            }

            return headerPosition;
        }

        private static UndoPropertyModification[] HandlePostprocessModifications(UndoPropertyModification[] modifications)
        {
            for (int i = 0; i < modifications.Length; i++)
            {
                ref UndoPropertyModification modification = ref modifications[i];

                if (!modification.currentValue.IsReorderableList(out ReorderableList list) || !typeof(ISerializableDictionary).IsAssignableFrom(list.serializedProperty.GetScopeInfo()!.PropertyType))
                {
                    continue;
                }

                PropertyPathInfo scope = list.serializedProperty.GetScopeInfo()!;

                Parallel.ForEach(list.GetTargetObjects(), delegate(Object target)
                {
                    scope.GetValue<ISerializableDictionary>(target)!.OnAfterDeserialize();
                });
            }

            return modifications;
        }

        private static void InitializeReorderableList(ReorderableList list)
        {
            static void add(ReorderableList list)
            {
                Object target = list.GetTargetObject();
                Undo.RecordObject(target, "Add Element To Dictionary");
                list.GrabKeyboardFocus();

                PropertyPathInfo scope = list.serializedProperty.GetScopeInfo()!;
                scope.GetValue<ISerializableDictionary>(target)!.Add();
                list.GetSerializedObject().ApplyModifiedPropertiesWithoutUndo();
                list.GetSerializedObject().UpdateIfRequiredOrScript();

                MethodInfo methodInfo = target.GetType().FindMethodByName("OnValidate");
                methodInfo?.Invoke(target, null);
            }

            static bool canAdd(ReorderableList list)
            {
                if (list.GetSerializedObject().isEditingMultipleObjects)
                {
                    return false;
                }

                PropertyPathInfo scope = list.serializedProperty.GetScopeInfo()!;

                return GUI.enabled && scope.FieldInfo.GetCustomAttribute<DisableResizeAttribute>() == null && scope.GetValue(list.GetTargetObject()) is ISerializableDictionary { CanAdd: true };
            }

            static void drawNone(Rect position)
            {
                EditorGUI.LabelField(position, "Dictionary is Empty");
            }

            static void remove(ReorderableList list)
            {
                ReorderableList.defaultBehaviours.DoRemoveButton(list);

                PropertyPathInfo scope = list.serializedProperty.GetScopeInfo()!;

                Parallel.ForEach(list.GetTargetObjects(), delegate(Object target)
                {
                    scope.GetValue<ISerializableDictionary>(target)!.OnAfterDeserialize();
                });
            }

#if UNITY_2021_3_OR_NEWER
            list.multiSelect = true;
#endif
            list.draggable = GUI.enabled;
            list.headerHeight = 0;
            list.drawNoneElementCallback = drawNone;
            list.onAddCallback = add;
            list.onCanAddCallback = canAdd;
            list.onRemoveCallback = remove;

            list.drawElementCallback = delegate(Rect position, int index, bool active, bool focused)
            {
                SerializedProperty elementProperty = list.serializedProperty.GetArrayElementAtIndex(index);
                float foldoutWidth = EditorStyles.foldout.CalcSize(GUIContent.none).x;
                position.xMin += foldoutWidth;
                position.yMin += EditorGUIUtility.standardVerticalSpacing;

                SerializedProperty keyProperty = elementProperty.FindPropertyRelative(KeyProperty);
                SerializedProperty valueProperty = elementProperty.FindPropertyRelative(ValueProperty);
                Rect keyPosition = position.WithHeight(EditorGUI.GetPropertyHeight(keyProperty, GUIContent.none));
                Rect valuePosition = position.WithHeight(EditorGUI.GetPropertyHeight(valueProperty, GUIContent.none));
                keyPosition.width = EditorGUIUtility.labelWidth - foldoutWidth - EditorGUIUtility.standardVerticalSpacing;
                valuePosition.xMin += EditorGUIUtility.labelWidth;

                using (new LabelWidthScope(keyPosition.width * LabelWidthPercent, LabelWidthScope.MagnitudeMode.Absolute))
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EditorGUI.PropertyField(keyPosition, keyProperty, true);
                    }
                }

                using (new LabelWidthScope(valuePosition.width * LabelWidthPercent, LabelWidthScope.MagnitudeMode.Absolute))
                {
                    EditorGUI.PropertyField(valuePosition, valueProperty, true);
                }
            };

            list.elementHeightCallback = delegate(int index)
            {
                if (list.serializedProperty.arraySize == 0)
                {
                    return 0;
                }

                SerializedProperty elementProperty = list.serializedProperty.GetArrayElementAtIndex(index);
                float keyPropertyHeight = EditorGUI.GetPropertyHeight(elementProperty.FindPropertyRelative(KeyProperty), GUIContent.none);
                float valuePropertyHeight = EditorGUI.GetPropertyHeight(elementProperty.FindPropertyRelative(ValueProperty), GUIContent.none);

                return Mathf.Max(keyPropertyHeight, valuePropertyHeight);
            };
        }
    }
}
