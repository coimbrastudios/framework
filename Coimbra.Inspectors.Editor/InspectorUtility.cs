#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Coimbra.Inspectors.Editor
{
    [InitializeOnLoad]
    public static class InspectorUtility
    {
        private static readonly Dictionary<Type, IInspectorDecoratorDrawer> DecoratorDrawerMap = new();

        static InspectorUtility()
        {
            InspectorEditorBase.DrawCustomInspectorHandler = DrawCustomInspector;
            DecoratorDrawerMap.Clear();

            foreach (Type decoratorDrawerType in TypeCache.GetTypesDerivedFrom<IInspectorDecoratorDrawer>())
            {
                if (decoratorDrawerType.IsAbstract || !decoratorDrawerType.IsDefined(typeof(InspectorDecoratorDrawerAttribute)))
                {
                    continue;
                }

                IInspectorDecoratorDrawer decoratorDrawer = (IInspectorDecoratorDrawer)Activator.CreateInstance(decoratorDrawerType);

                foreach (InspectorDecoratorDrawerAttribute decoratorDrawerAttribute in decoratorDrawerType.GetCustomAttributes<InspectorDecoratorDrawerAttribute>())
                {
                    if (!decoratorDrawerAttribute.Type.IsSubclassOf(typeof(InspectorDecoratorAttributeBase)))
                    {
                        Debug.LogError($"{nameof(InspectorDecoratorDrawerAttribute)}.{nameof(InspectorDecoratorDrawerAttribute.Type)} expects a type that inherits from {nameof(InspectorDecoratorAttributeBase)}!");

                        continue;
                    }

                    DecoratorDrawerMap[decoratorDrawerAttribute.Type] = decoratorDrawer;

                    if (!decoratorDrawerAttribute.UseForChildren)
                    {
                        continue;
                    }

                    foreach (Type derivedType in TypeCache.GetTypesDerivedFrom(decoratorDrawerAttribute.Type))
                    {
                        if (!DecoratorDrawerMap.ContainsKey(derivedType))
                        {
                            DecoratorDrawerMap.Add(derivedType, decoratorDrawer);
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckConditions(string? predicate, DecoratorConditions? conditions)
        {
            // TODO

            return true;
        }

        /// <summary>
        /// Draws all members that contains <see cref="ShowInInspectorAttribute"/>.
        /// </summary>
        /// <param name="scopes">The list of targets being drawn.</param>
        /// <param name="memberKinds">Member kinds to draw.</param>
        /// <returns>The total height of the drawn GUI.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawMembersWithShowInInspector(IReadOnlyList<Object> scopes, ShowInInspectorMemberKinds memberKinds)
        {
            Rect position = EditorGUILayout.GetControlRect(false, 0);
            float totalHeight = DrawMembersWithShowInInspector(position, scopes, memberKinds);
            EditorGUILayout.GetControlRect(false, totalHeight);
        }

        /// <summary>
        /// Draws all members that contains <see cref="ShowInInspectorAttribute"/>.
        /// </summary>
        /// <param name="position">The initial GUI position.</param>
        /// <param name="scopes">The list of targets being drawn.</param>
        /// <param name="memberKinds">Member kinds to draw.</param>
        /// <returns>The total height of the drawn GUI.</returns>
        public static float DrawMembersWithShowInInspector(Rect position, IReadOnlyList<Object> scopes, ShowInInspectorMemberKinds memberKinds)
        {
            Type type = scopes[0].GetType();
            InspectorCache inspectorCache = InspectorCache.Get(type);
            float totalHeight = 0;
            position.height = 0;

            if ((memberKinds & ShowInInspectorMemberKinds.Static) != 0)
            {
                for (int i = 0; i < inspectorCache.StaticMembersWithShowInInspector.Count; i++)
                {
                    InspectorMember inspectorMember = inspectorCache.StaticMembersWithShowInInspector[i];

                    if (!CheckConditions(inspectorMember.ShowInInspectorAttribute!.Predicate, inspectorMember.ShowInInspectorAttribute.Conditions))
                    {
                        continue;
                    }

                    totalHeight += Draw(ref position, inspectorMember, scopes, null);
                    totalHeight += EditorGUIUtility.standardVerticalSpacing;
                }
            }

            if ((memberKinds & ShowInInspectorMemberKinds.Instance) != 0)
            {
                for (int i = 0; i < inspectorCache.InstanceMembersWithShowInInspector.Count; i++)
                {
                    InspectorMember inspectorMember = inspectorCache.InstanceMembersWithShowInInspector[i];

                    if (!CheckConditions(inspectorMember.ShowInInspectorAttribute!.Predicate, inspectorMember.ShowInInspectorAttribute.Conditions))
                    {
                        continue;
                    }

                    totalHeight += Draw(ref position, inspectorMember, scopes, null);
                    totalHeight += EditorGUIUtility.standardVerticalSpacing;
                }
            }

            return totalHeight;
        }

        /// <summary>
        /// Draws a <see cref="SerializedProperty"/> while processing all of its <see cref="InspectorDecoratorAttributeBase"/>.
        /// </summary>
        /// <param name="serializedProperty">The property being drawn.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawSerializedProperty(SerializedProperty serializedProperty)
        {
            Rect position = EditorGUILayout.GetControlRect(false, 0);
            float totalHeight = DrawSerializedProperty(position, serializedProperty);
            EditorGUILayout.GetControlRect(false, totalHeight);
        }

        /// <summary>
        /// Draws a <see cref="SerializedProperty"/> while processing all of its <see cref="InspectorDecoratorAttributeBase"/>.
        /// </summary>
        /// <param name="position">The initial GUI position.</param>
        /// <param name="serializedProperty">The property being drawn.</param>
        /// <returns>The total height of the drawn GUI.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DrawSerializedProperty(Rect position, SerializedProperty serializedProperty)
        {
            IReadOnlyList<Object> scopes = serializedProperty.serializedObject.targetObjects;
            InspectorCache inspectorCache = InspectorCache.Get(scopes[0].GetType());

            if (inspectorCache.Members.TryGetValue(InspectorMemberId.Get(serializedProperty), out InspectorMember inspectorMember))
            {
                if (CheckConditions(inspectorMember.HideInInspectorIfAttribute!.Predicate, inspectorMember.HideInInspectorIfAttribute.Conditions))
                {
                    return 0;
                }

                return Draw(ref position, inspectorMember, scopes, serializedProperty);
            }

            position.height = EditorGUI.GetPropertyHeight(serializedProperty, true);
            EditorGUI.PropertyField(position, serializedProperty, true);

            return position.height;
        }

        private static float Draw(ref Rect position, InspectorMember member, IReadOnlyList<Object> scopes, SerializedProperty? serializedProperty)
        {
            float totalHeight = 0;

            InspectorDecoratorDrawerContext context = new()
            {
                Label = member.Label,
                MemberInfo = member.MemberInfo,
                Scopes = scopes,
                SerializedProperty = serializedProperty,
            };

            position.height = 0;

            for (int i = 0; i < member.DecoratorAttributes.Count; i++)
            {
                context.Attribute = member.DecoratorAttributes[i];

                if (!DecoratorDrawerMap.TryGetValue(context.Attribute.GetType(), out IInspectorDecoratorDrawer drawer))
                {
                    continue;
                }

                context.Position = position;
                position.height = drawer.GetHeightBeforeGUI(ref context);
                context.Position = position;
                drawer.OnBeforeGUI(ref context);

                totalHeight += position.height;
                position.y += position.height;
                position.height = 0;
            }

            if (serializedProperty != null)
            {
                position.height = EditorGUI.GetPropertyHeight(serializedProperty, context.Label, true);
                EditorGUI.PropertyField(position, serializedProperty, context.Label, true);

                totalHeight += position.height;
                position.y += position.height;
                position.height = 0;
            }
            else
            {
                position.height = EditorGUIUtility.singleLineHeight;

                switch (member.MemberInfo)
                {
                    case FieldInfo fieldInfo:
                    {
                        EditorGUI.LabelField(position, context.Label.text, fieldInfo.GetValue(scopes[0]).ToString());

                        break;
                    }

                    case MethodInfo methodInfo:
                    {
                        EditorGUI.LabelField(position, context.Label.text, methodInfo.Invoke(scopes[0], null).ToString());

                        break;
                    }

                    case PropertyInfo propertyInfo:
                    {
                        EditorGUI.LabelField(position, context.Label.text, propertyInfo.GetValue(scopes[0]).ToString());

                        break;
                    }
                }

                totalHeight += position.height;
                position.y += position.height;
                position.height = 0;
            }

            for (int i = member.DecoratorAttributes.Count - 1; i >= 0; i--)
            {
                context.Attribute = member.DecoratorAttributes[i];

                if (!DecoratorDrawerMap.TryGetValue(context.Attribute.GetType(), out IInspectorDecoratorDrawer drawer))
                {
                    continue;
                }

                context.Position = position;
                position.height = drawer.GetHeightAfterGUI(ref context);
                context.Position = position;
                drawer.OnAfterGUI(ref context);

                totalHeight += position.height;
                position.y += position.height;
                position.height = 0;
            }

            return totalHeight;
        }

        private static void DrawCustomInspector(InspectorEditorBase inspectorEditor)
        {
            inspectorEditor.serializedObject.UpdateIfRequiredOrScript();

            SerializedProperty iterator = inspectorEditor.serializedObject.GetIterator();

            if (iterator.NextVisible(true))
            {
                if (inspectorEditor.DrawScriptField)
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EditorGUILayout.PropertyField(iterator, true);
                    }
                }

                using (new GUILayout.VerticalScope(GUI.skin.box))
                {
                    DrawMembersWithShowInInspector(inspectorEditor.targets, ShowInInspectorMemberKinds.Static);
                }

                while (iterator.NextVisible(false))
                {
                    bool enabled = GUI.enabled;
                    int indentLevel = EditorGUI.indentLevel;
                    DrawSerializedProperty(iterator);

                    GUI.enabled = enabled;
                    EditorGUI.indentLevel = indentLevel;
                }

                if (!inspectorEditor.serializedObject.isEditingMultipleObjects)
                {
                    using (new GUILayout.VerticalScope(GUI.skin.box))
                    {
                        DrawMembersWithShowInInspector(inspectorEditor.targets, ShowInInspectorMemberKinds.Instance);
                    }
                }
            }

            inspectorEditor.serializedObject.ApplyModifiedProperties();
        }
    }
}
