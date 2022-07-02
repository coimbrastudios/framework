using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Drawer for <see cref="ValidateAttribute"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(ValidateAttribute), false)]
    public class ValidateDrawer : PropertyDrawer
    {
        /// <inheritdoc/>
        public sealed override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            PropertyPathInfo propertyPathInfo = property.GetPropertyPathInfo();
            ValidateAttribute validateAttribute = propertyPathInfo.FieldInfo.GetCustomAttribute<ValidateAttribute>();
            Object[] targets = property.serializedObject.targetObjects;

            if (string.IsNullOrWhiteSpace(validateAttribute?.Callback))
            {
                DrawGUI(position, property, label, propertyPathInfo, targets, validateAttribute?.Delayed ?? false);

                return;
            }

            using (ListPool.Pop(out List<object> scopes))
            {
                propertyPathInfo.GetScopes(targets, scopes);

                MethodInfo methodInfo = scopes[0].GetType().FindMethodBySignature(validateAttribute.Callback) ?? scopes[0].GetType().FindMethodBySignature(validateAttribute.Callback, propertyPathInfo.PropertyType);

                using EditorGUI.ChangeCheckScope changeCheckScope = new();

                if (methodInfo == null)
                {
                    DrawGUI(position, property, label, propertyPathInfo, targets, validateAttribute.Delayed);

                    return;
                }

                if (methodInfo.GetParameters().Length == 0)
                {
                    DrawGUI(position, property, label, propertyPathInfo, targets, validateAttribute.Delayed);

                    if (!changeCheckScope.changed)
                    {
                        return;
                    }

                    foreach (object scope in scopes)
                    {
                        methodInfo.Invoke(scope, null);
                    }
                }
                else
                {
                    using (ListPool.Pop(out List<object> values))
                    {
                        propertyPathInfo.GetValues(targets, values);
                        DrawGUI(position, property, label, propertyPathInfo, targets, validateAttribute.Delayed);

                        if (!changeCheckScope.changed)
                        {
                            return;
                        }

                        object[] parameters = new object[1];

                        for (int i = 0; i < targets.Length; i++)
                        {
                            parameters[0] = values[i];
                            methodInfo.Invoke(scopes[i], parameters);
                        }
                    }
                }
            }
        }

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return CoimbraGUIUtility.GetPropertyHeight(property, label);
        }

        /// <inheritdoc cref="OnGUI"/>
        protected virtual void DrawGUI(Rect position, SerializedProperty property, GUIContent label, PropertyPathInfo context, Object[] targets, bool isDelayed)
        {
            if (isDelayed)
            {
                switch (property.propertyType)
                {
                    case SerializedPropertyType.Float:
                    {
                        EditorGUI.DelayedFloatField(position, property, label);

                        break;
                    }

                    case SerializedPropertyType.Integer:
                    {
                        EditorGUI.DelayedIntField(position, property, label);

                        break;
                    }

                    case SerializedPropertyType.String:
                    {
                        EditorGUI.DelayedTextField(position, property, label);

                        break;
                    }

                    default:
                    {
                        CoimbraGUIUtility.DrawPropertyField(position, property, label);

                        break;
                    }
                }
            }
            else
            {
                CoimbraGUIUtility.DrawPropertyField(position, property, label);
            }
        }
    }
}
