using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Drawer for <see cref="TypeDropdownDrawer"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(TypeDropdownAttribute))]
    public sealed class TypeDropdownDrawer : PropertyDrawer
    {
        private const string ChangeUndoKey = "Change Type Value";

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect valuePosition = position;
            position.xMin += EditorGUIUtility.labelWidth;
            position.height = EditorGUIUtility.singleLineHeight;

            using (GUIContentPool.Pop(out GUIContent typeLabel))
            {
                string typeName = property.managedReferenceFullTypename;
                typeLabel.text = string.IsNullOrWhiteSpace(typeName) ? "<null>" : TypeString.Get(TypeUtility.GetType(typeName));
                typeLabel.tooltip = typeLabel.text;

                TypeDropdown.DrawReferenceField(position, property.GetFieldInfo().FieldType, property, typeLabel, ChangeUndoKey, delegate(List<Type> list)
                {
                    TypeDropdown.FilterTypes(property.serializedObject.targetObjects, property.GetPropertyPathInfo(), list);
                });
            }

            using EditorGUI.PropertyScope propertyScope = new EditorGUI.PropertyScope(valuePosition, label, property);
            EditorGUI.PropertyField(valuePosition, property, propertyScope.content, true);
        }
    }
}
