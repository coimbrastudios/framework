using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Coimbra.Editor
{
    /// <summary>
    /// Drawer for <see cref="TypeDropdownDrawer"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(TypeDropdownAttribute))]
    public sealed class TypeDropdownDrawer : ValidateDrawer
    {
        private const string ChangeUndoKey = "Change Type Value";

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        /// <inheritdoc/>
        protected override void DrawGUI(Rect position, SerializedProperty property, GUIContent label, PropertyPathInfo context, Object[] targets, bool isDelayed)
        {
            Rect valuePosition = position;
            position.height = EditorGUIUtility.singleLineHeight;
            CoimbraGUIUtility.AdjustPosition(ref position, InspectorArea.Field);

            using (GUIContentPool.Pop(out GUIContent typeLabel))
            {
                string typeName = property.managedReferenceFullTypename;
                typeLabel.text = string.IsNullOrWhiteSpace(typeName) ? "<null>" : TypeString.Get(TypeUtility.GetType(typeName));
                typeLabel.tooltip = typeLabel.text;

                if (context.Depth > 0 && context.ScopeInfo!.PropertyType.IsGenericType && context.ScopeInfo.PropertyType.GetGenericTypeDefinition() == typeof(ManagedField<>))
                {
                    TypeDropdown.DrawReferenceField(position, context.ScopeInfo.PropertyType.GenericTypeArguments[0], property, typeLabel, ChangeUndoKey, delegate(List<Type> list)
                    {
                        TypeDropdown.FilterTypes(targets, context.ScopeInfo, list);
                    });
                }
                else
                {
                    TypeDropdown.DrawReferenceField(position, property.GetPropertyType(), property, typeLabel, ChangeUndoKey, delegate(List<Type> list)
                    {
                        TypeDropdown.FilterTypes(targets, context, list);
                    });
                }
            }

            using EditorGUI.PropertyScope propertyScope = new(valuePosition, label, property);
            EditorGUI.PropertyField(valuePosition, property, propertyScope.content, true);
        }
    }
}
