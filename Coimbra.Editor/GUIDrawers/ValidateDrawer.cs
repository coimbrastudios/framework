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
        private static readonly object[] InvokeParameters = new object[1];

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
            using (ListPool.Pop(out List<object> values))
            {
                propertyPathInfo.GetScopes(targets, scopes, true);
                propertyPathInfo.GetValues(targets, values);

                using EditorGUI.ChangeCheckScope changeCheckScope = new();

                DrawGUI(position, property, label, propertyPathInfo, targets, validateAttribute.Delayed);

                if (!changeCheckScope.changed)
                {
                    return;
                }

                for (int i = 0; i < targets.Length; i++)
                {
                    MethodInfo methodInfo = scopes[i].GetType().FindMethodBySignature(validateAttribute.Callback);

                    if (methodInfo != null)
                    {
                        methodInfo.Invoke(scopes[i], null);
                    }
                    else
                    {
                        methodInfo = scopes[i].GetType().FindMethodBySignature(validateAttribute.Callback, propertyPathInfo.PropertyType);

                        if (methodInfo == null)
                        {
                            continue;
                        }

                        InvokeParameters[0] = values[i];
                        methodInfo.Invoke(scopes[i], InvokeParameters);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EngineUtility.GetPropertyHeight(property, label);
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
                        EngineUtility.DrawPropertyField(position, property, label);

                        break;
                    }
                }
            }
            else
            {
                EngineUtility.DrawPropertyField(position, property, label);
            }
        }
    }
}
