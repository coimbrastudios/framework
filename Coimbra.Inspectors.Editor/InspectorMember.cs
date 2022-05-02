#nullable enable

using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Inspectors.Editor
{
    internal sealed class InspectorMember
    {
        internal readonly MemberInfo MemberInfo;

        internal readonly GUIContent Label;

        internal readonly ShowInInspectorAttribute? ShowInInspectorAttribute;

        internal readonly HideInInspectorIfAttribute? HideInInspectorIfAttribute;

        internal readonly IReadOnlyList<InspectorDecoratorAttributeBase> DecoratorAttributes;

        internal InspectorMember(MemberInfo memberInfo,
                                 GUIContent label,
                                 ShowInInspectorAttribute? showInInspectorAttribute,
                                 HideInInspectorIfAttribute? hideInInspectorIfAttribute,
                                 IReadOnlyList<InspectorDecoratorAttributeBase> decoratorAttributes)

        {
            MemberInfo = memberInfo;
            Label = label;
            ShowInInspectorAttribute = showInInspectorAttribute;
            HideInInspectorIfAttribute = hideInInspectorIfAttribute;
            DecoratorAttributes = decoratorAttributes;
        }

        internal float DrawGUI(Rect position, IReadOnlyList<object> scopes, SerializedProperty? serializedProperty, bool includeChildren)
        {
            float totalHeight = 0;

            InspectorDecoratorDrawerContext context = new()
            {
                Label = Label,
                MemberInfo = MemberInfo,
                Scopes = scopes,
                SerializedProperty = serializedProperty,
            };

            for (int i = 0; i < DecoratorAttributes.Count; i++)
            {
                context.Attribute = DecoratorAttributes[i];

                float height = InspectorDecoratorDrawerUtility.DrawBeforeGUI(position, ref context);
                totalHeight += height;
                position.y += height;
            }

            if (serializedProperty != null)
            {
                position.height = EditorGUI.GetPropertyHeight(serializedProperty, context.Label, includeChildren);
                EditorGUI.PropertyField(position, serializedProperty, context.Label, includeChildren);

                totalHeight += position.height;
                position.y += position.height;
            }
            else
            {
                position.height = EditorGUIUtility.singleLineHeight;
                DrawMemberInfoField(ref position, ref context);

                totalHeight += position.height;
                position.y += position.height;
            }

            for (int i = DecoratorAttributes.Count - 1; i >= 0; i--)
            {
                context.Attribute = DecoratorAttributes[i];

                float height = InspectorDecoratorDrawerUtility.DrawAfterGUI(position, ref context);
                totalHeight += height;
                position.y += height;
            }

            return totalHeight;
        }

        internal float GetGUIHeight(IReadOnlyList<object> scopes, SerializedProperty? serializedProperty, bool includeChildren)
        {
            float totalHeight = 0;

            InspectorDecoratorDrawerContext context = new()
            {
                Label = Label,
                MemberInfo = MemberInfo,
                Scopes = scopes,
                SerializedProperty = serializedProperty,
            };

            for (int i = 0; i < DecoratorAttributes.Count; i++)
            {
                context.Attribute = DecoratorAttributes[i];
                totalHeight += InspectorDecoratorDrawerUtility.GetBeforeGUIHeight(ref context);
            }

            if (serializedProperty != null)
            {
                totalHeight += EditorGUI.GetPropertyHeight(serializedProperty, context.Label, includeChildren);
            }
            else
            {
                totalHeight += EditorGUIUtility.singleLineHeight;
            }

            for (int i = DecoratorAttributes.Count - 1; i >= 0; i--)
            {
                context.Attribute = DecoratorAttributes[i];
                totalHeight += InspectorDecoratorDrawerUtility.GetAfterGUIHeight(ref context);
            }

            return totalHeight;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DrawMemberInfoField(ref Rect position, ref InspectorDecoratorDrawerContext context)
        {
            switch (context.MemberInfo)
            {
                case FieldInfo fieldInfo:
                {
                    EditorGUI.LabelField(position, context.Label.text, fieldInfo.GetValue(context.Scopes[0]).ToString());

                    break;
                }

                case MethodInfo methodInfo:
                {
                    EditorGUI.LabelField(position, context.Label.text, methodInfo.Invoke(context.Scopes[0], null).ToString());

                    break;
                }

                case PropertyInfo propertyInfo:
                {
                    EditorGUI.LabelField(position, context.Label.text, propertyInfo.GetValue(context.Scopes[0]).ToString());

                    break;
                }
            }
        }
    }
}
