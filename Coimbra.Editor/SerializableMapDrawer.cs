using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Drawer for <see cref="SerializableMap{TKey,TValue}"/>.
    /// </summary>
    [InitializeOnLoad]
    [CustomPropertyDrawer(typeof(SerializableMap<,>), true)]
    public class SerializableMapDrawer : PropertyDrawer
    {
        private const float CountFieldSize = 50;

        private const string KeyProperty = "Key";

        private const string PairProperty = "_pair";

        private const string PairsProperty = "_pairs";

        private const string ValueProperty = "Value";

        private const string ModifyDisabledMessage = "Can't resize map while editing multiple objects!";

        static SerializableMapDrawer()
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

            SerializedProperty pairsProperty = property.FindPropertyRelative(PairsProperty);

            return height + EditorGUIUtility.standardVerticalSpacing + pairsProperty.ToReorderableList(InitializeReorderableList).GetHeight();
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool isEditingMultipleObjects = property.serializedObject.isEditingMultipleObjects;
            Rect headerPosition = position;
            headerPosition.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(headerPosition, property, label, false);

            SerializedProperty pairsProperty = property.FindPropertyRelative(PairsProperty);

            using (new EditorGUI.DisabledScope(true))
            {
                headerPosition.xMin += headerPosition.width - CountFieldSize;

                static bool shouldShowMixedValue(SerializedProperty property)
                {
                    using (SharedManagedPools.Pop(out List<ICollection> collections))
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

                using (new ShowMixedValueScope(isEditingMultipleObjects && shouldShowMixedValue(pairsProperty)))
                {
                    EditorGUI.IntField(headerPosition, pairsProperty.arraySize);
                }
            }

            if (!property.isExpanded)
            {
                return;
            }

            ReorderableList list = pairsProperty.ToReorderableList(InitializeReorderableList);
            position.yMin += headerPosition.height + EditorGUIUtility.standardVerticalSpacing;
            list.DoList(position);

            if (CanModifyList(list))
            {
                return;
            }

            position.yMin += position.height - EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(position, ModifyDisabledMessage);
        }

        private static bool CanModifyList(ReorderableList list)
        {
            return !list.serializedProperty.serializedObject.isEditingMultipleObjects;
        }

        private static void DrawKeyValuePair(Rect position, SerializedProperty property, bool disableKeyField)
        {
            static void draw(Rect position, SerializedProperty property)
            {
                if (property.isArray)
                {
                    EditorGUI.PropertyField(position, property);
                }
                else
                {
                    EditorGUI.PropertyField(position, property, GUIContent.none);
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

        private static float GetKeyValuePairHeight(SerializedProperty property)
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

                if (modification.currentValue.IsReorderableList(out ReorderableList list))
                {
                    list.serializedProperty.GetScope<ISerializableMap>()!.ProcessUndo();
                }
            }

            return modifications;
        }

        private static void InitializeReorderableList(ReorderableList list)
        {
            static void drawNoneElement(Rect position)
            {
                EditorGUI.LabelField(position, "Map is Empty");
            }

            static void handleAddDropdown(Rect position, ReorderableList list)
            {
                Vector2 windowScrollPosition = new();
                string mapPropertyPath = list.serializedProperty.propertyPath;
                mapPropertyPath = mapPropertyPath[..mapPropertyPath.LastIndexOf('.')];

                SerializedObject serializedObject = list.serializedProperty.serializedObject;
                SerializedProperty mapProperty = serializedObject.FindProperty(mapPropertyPath);
                SerializedProperty pairProperty = mapProperty.FindPropertyRelative(PairProperty);

                void guiCallback(TemporaryWindow window)
                {
                    using (new LabelWidthScope(EditorGUIUtility.currentViewWidth * 0.5f, LabelWidthScope.MagnitudeMode.Absolute))
                    {

                        using EditorGUILayout.ScrollViewScope scrollViewScope = new(windowScrollPosition);
                        windowScrollPosition = scrollViewScope.scrollPosition;

                        ISerializableMap serializableMap = list.serializedProperty.GetScope<ISerializableMap>();

                        if (serializableMap == null)
                        {
                            window.Close();

                            return;
                        }

                        EditorGUILayout.LabelField($"Key ({serializableMap.KeyType})", $"Value ({serializableMap.ValueType})");

                        using EditorGUI.ChangeCheckScope changeCheckScope = new();
                        Rect position = EditorGUILayout.GetControlRect(false, GetKeyValuePairHeight(pairProperty));
                        DrawKeyValuePair(position, pairProperty, false);

                        if (changeCheckScope.changed)
                        {
                            serializedObject.ApplyModifiedProperties();
                            serializedObject.UpdateIfRequiredOrScript();
                        }

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            GUILayout.FlexibleSpace();

                            using (new EditorGUI.DisabledScope(!serializableMap.IsPairValid))
                            {
                                if (GUILayout.Button("Add"))
                                {
                                    Undo.RecordObject(serializedObject.targetObject, "Add Element To Map");
                                    serializableMap.ProcessAdd();
                                    serializedObject.ApplyModifiedPropertiesWithoutUndo();
                                    serializedObject.UpdateIfRequiredOrScript();
                                    EditorUtility.SetDirty(serializedObject.targetObject);
                                    window.Close();
                                }
                            }

                            if (GUILayout.Button("Cancel"))
                            {
                                window.Close();
                            }
                        }
                    }
                }

                float windowHeight = GetKeyValuePairHeight(pairProperty) + EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing * 6;
                string windowTitle = $"Add Element To {mapProperty.displayName}";
                position.size = new Vector2(EditorGUIUtility.currentViewWidth, windowHeight);

                TemporaryWindow window = TemporaryWindow.Get(windowTitle, guiCallback);
                window.position = position;
            }

            void drawHeader(Rect position)
            {
                float labelWidth = EditorGUIUtility.labelWidth;
                ISerializableMap serializableMap = list.serializedProperty.GetScope<ISerializableMap>()!;
                EditorGUIUtility.labelWidth += ReorderableList.Defaults.dragHandleWidth * 0.5f;
                EditorGUI.LabelField(position, $"Keys ({serializableMap.KeyType})", $"Values ({serializableMap.ValueType})");

                EditorGUIUtility.labelWidth = labelWidth;
            }

            float getElementHeight(int index)
            {
                if (list.serializedProperty.arraySize == 0)
                {
                    return 0;
                }

                SerializedProperty elementProperty = list.serializedProperty.GetArrayElementAtIndex(index);

                return GetKeyValuePairHeight(elementProperty) + EditorGUIUtility.standardVerticalSpacing;
            }

            list.multiSelect = true;
            list.drawHeaderCallback = drawHeader;
            list.drawNoneElementCallback = drawNoneElement;
            list.elementHeightCallback = getElementHeight;
            list.onAddDropdownCallback = handleAddDropdown;
            list.onCanAddCallback = CanModifyList;
            list.onCanRemoveCallback = CanModifyList;

            list.drawElementCallback = delegate(Rect position, int index, bool _, bool _)
            {
                Rect drawPosition = position;
                drawPosition.yMin += EditorGUIUtility.standardVerticalSpacing;

                SerializedProperty elementProperty = list.serializedProperty.GetArrayElementAtIndex(index);
                DrawKeyValuePair(drawPosition, elementProperty, true);
            };
        }
    }
}
