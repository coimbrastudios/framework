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
        private const float KeyWidthPercent = 0.4f;

        private const float LabelWidthPercent = 0.5f;

        private const string KeyProperty = "_key";

        private const string ListProperty = "_list";

        private const string ModifyDisabledMessage = "Can't add new item while editing multiple objects!";

        private const string NewProperty = "_new";

        private const string ValueProperty = "_value";

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
            FieldInfo scopeFieldInfo = list.serializedProperty.GetScopeInfo()!.FieldInfo;
            list.draggable = GUI.enabled && scopeFieldInfo.GetCustomAttribute<DisableAttribute>() == null;

            bool canResize = list.draggable && scopeFieldInfo.GetCustomAttribute<DisableResizeAttribute>() == null;
            list.displayAdd = canResize;
            list.displayRemove = canResize;
            list.footerHeight = canResize ? EditorGUIUtility.singleLineHeight : 0;
            height += EditorGUIUtility.standardVerticalSpacing + list.GetHeight();

            if (!canResize)
            {
                return height;
            }

            SerializedProperty newProperty = property.FindPropertyRelative(NewProperty);
            height += EditorGUIUtility.standardVerticalSpacing + EditorGUI.GetPropertyHeight(newProperty, newProperty.isExpanded) - EditorGUIUtility.singleLineHeight;

            return height;
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ReorderableList list = property.FindPropertyRelative(ListProperty).ToReorderableList(InitializeReorderableList);
            CoimbraGUIUtility.DrawListHeader(position, label, property, list);

            if (!property.isExpanded)
            {
                return;
            }

            FieldInfo scopeFieldInfo = list.serializedProperty.GetScopeInfo()!.FieldInfo;
            list.draggable = GUI.enabled && scopeFieldInfo.GetCustomAttribute<DisableAttribute>() == null;

            if (!list.draggable)
            {
                position.xMin += ReorderableList.Defaults.dragHandleWidth;
            }

            Rect listPosition = position;
            bool canResize = list.draggable && scopeFieldInfo.GetCustomAttribute<DisableResizeAttribute>() == null;
            list.displayAdd = canResize;
            list.displayRemove = canResize;
            list.footerHeight = canResize ? EditorGUIUtility.singleLineHeight : 0;
            listPosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            listPosition.height = list.GetHeight();

            using (new LabelWidthScope(listPosition.width * KeyWidthPercent, LabelWidthScope.MagnitudeMode.Absolute))
            {
                list.DoList(listPosition);
            }

            if (!canResize)
            {
                return;
            }

            position.y += EditorGUIUtility.standardVerticalSpacing + listPosition.height;

            if (property.serializedObject.isEditingMultipleObjects)
            {
                position.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(position, ModifyDisabledMessage);

                return;
            }

            SerializedProperty newProperty = property.FindPropertyRelative(NewProperty);
            float foldoutWidth = EditorStyles.foldout.CalcSize(GUIContent.none).x;
            CoimbraGUIUtility.AdjustPosition(ref position, InspectorArea.Label);

            if (list.draggable)
            {
                position.x += ReorderableList.Defaults.dragHandleWidth;
            }

            position.xMin += foldoutWidth;
            position.height = EditorGUI.GetPropertyHeight(newProperty, true);

            using (new ResetIndentLevelScope())
            using (new LabelWidthScope(position.width * LabelWidthPercent, LabelWidthScope.MagnitudeMode.Absolute))
            {
                DrawElement(position, newProperty, list.serializedProperty.GetScopeInfo()!.FieldInfo.IsDefined(typeof(HideKeyLabelAttribute)));
            }
        }

        private static void DrawElement(Rect position, SerializedProperty property, bool forceHideLabel)
        {
            if (forceHideLabel)
            {
                EditorGUI.PropertyField(position, property, GUIContent.none, true);

                return;
            }

            switch (property.propertyType)
            {
                case SerializedPropertyType.Boolean:
                case SerializedPropertyType.Float:
                case SerializedPropertyType.String:
                case SerializedPropertyType.Color:
                case SerializedPropertyType.ObjectReference:
                case SerializedPropertyType.LayerMask:
                case SerializedPropertyType.Enum:
                case SerializedPropertyType.Vector2:
                case SerializedPropertyType.Vector3:
                case SerializedPropertyType.Rect:
                case SerializedPropertyType.Integer:
                case SerializedPropertyType.Character:
                case SerializedPropertyType.AnimationCurve:
                case SerializedPropertyType.Bounds:
                case SerializedPropertyType.Gradient:
                case SerializedPropertyType.Quaternion:
                case SerializedPropertyType.ExposedReference:
                case SerializedPropertyType.Vector2Int:
                case SerializedPropertyType.Vector3Int:
                case SerializedPropertyType.RectInt:
                case SerializedPropertyType.BoundsInt:
                {
                    EditorGUI.PropertyField(position, property, GUIContent.none, true);

                    break;
                }

                default:
                {
                    EditorGUI.PropertyField(position, property, true);

                    break;
                }
            }
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
            list.headerHeight = 0;
            list.drawNoneElementCallback = drawNone;
            list.onAddCallback = add;
            list.onCanAddCallback = canAdd;
            list.onRemoveCallback = remove;

            list.drawElementCallback = delegate(Rect position, int index, bool _, bool _)
            {
                SerializedProperty elementProperty = list.serializedProperty.GetArrayElementAtIndex(index);
                float foldoutWidth = EditorStyles.foldout.CalcSize(GUIContent.none).x;
                position.yMin += EditorGUIUtility.standardVerticalSpacing;

                SerializedProperty keyProperty = elementProperty.FindPropertyRelative(KeyProperty);
                SerializedProperty valueProperty = elementProperty.FindPropertyRelative(ValueProperty);
                Rect keyPosition = position.WithHeight(EditorGUI.GetPropertyHeight(keyProperty, GUIContent.none));
                Rect valuePosition = position.WithHeight(EditorGUI.GetPropertyHeight(valueProperty, GUIContent.none));
                CoimbraGUIUtility.AdjustPosition(ref keyPosition, InspectorArea.Label);
                CoimbraGUIUtility.AdjustPosition(ref valuePosition, InspectorArea.Field);
                keyPosition.xMin += foldoutWidth;
                valuePosition.xMin += foldoutWidth;

                using (new ResetIndentLevelScope())
                {
                    using (new LabelWidthScope(keyPosition.width * LabelWidthPercent, LabelWidthScope.MagnitudeMode.Absolute))
                    using (new EditorGUI.DisabledScope(true))
                    {
                        DrawElement(keyPosition, keyProperty, list.serializedProperty.GetScopeInfo()!.FieldInfo.IsDefined(typeof(HideKeyLabelAttribute)));
                    }

                    using (new LabelWidthScope(valuePosition.width * LabelWidthPercent, LabelWidthScope.MagnitudeMode.Absolute))
                    {
                        DrawElement(valuePosition, valueProperty, list.serializedProperty.GetScopeInfo()!.FieldInfo.IsDefined(typeof(HideValueLabelAttribute)));
                    }
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
