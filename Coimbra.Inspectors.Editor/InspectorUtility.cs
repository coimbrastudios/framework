#nullable enable

using Coimbra.Editor;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Inspectors.Editor
{
    [InitializeOnLoad]
    public static class InspectorUtility
    {
        static InspectorUtility()
        {
            InspectorEditorBase.DrawCustomInspectorsHandler = delegate(SerializedProperty iterator, bool includeChildren)
            {
                Rect position = EditorGUILayout.GetControlRect(false, 0);
                float totalHeight = DrawCustomInspectors(position, iterator, includeChildren);
                EditorGUILayout.GetControlRect(false, totalHeight);
            };

            InspectorPropertyDrawerBase.DrawCustomInspectorsHandler = delegate(Rect position, SerializedProperty iterator, bool includeChildren)
            {
                DrawCustomInspectors(position, iterator, includeChildren);
            };

            InspectorPropertyDrawerBase.GetCustomInspectorsHeightHandler = GetCustomInspectorHeight;
        }

        /// <summary>
        /// Checks a given expression and <see cref="DecoratorConditions"/>.
        /// </summary>
        /// <param name="predicate">The expression to check. Expects to represent a bool value.</param>
        /// <param name="conditions">The additional conditions to check.</param>
        /// <returns>True if both the expression and the conditions passes.</returns>
        public static bool CheckConditions(string? predicate, DecoratorConditions? conditions)
        {
            // TODO

            return true;
        }

        /// <summary>
        /// Draws all members that contains <see cref="ShowInInspectorAttribute"/>.
        /// </summary>
        /// <param name="position">The initial GUI position.</param>
        /// <param name="scopes">The list of targets being drawn.</param>
        /// <param name="memberKinds">Member kinds to draw.</param>
        /// <returns>The total height of the drawn GUI.</returns>
        public static float DrawMembersWithShowInInspector(Rect position, IReadOnlyList<object> scopes, ShowInInspectorMemberKinds memberKinds)
        {
            Type type = scopes[0].GetType();
            InspectorCache inspectorCache = InspectorCache.Get(type);
            float totalHeight = 0;

            if ((memberKinds & ShowInInspectorMemberKinds.Static) != 0)
            {
                for (int i = 0; i < inspectorCache.StaticMembersWithShowInInspector.Count; i++)
                {
                    InspectorMember inspectorMember = inspectorCache.StaticMembersWithShowInInspector[i];

                    if (!CheckConditions(inspectorMember.ShowInInspectorAttribute!.Predicate, inspectorMember.ShowInInspectorAttribute.Conditions))
                    {
                        continue;
                    }

                    float height = inspectorMember.DrawGUI(position, scopes, null, false);

                    if (height > 0)
                    {
                        position.y += height + EditorGUIUtility.standardVerticalSpacing;
                        totalHeight += height + EditorGUIUtility.standardVerticalSpacing;
                    }
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

                    float height = inspectorMember.DrawGUI(position, scopes, null, false);

                    if (height > 0)
                    {
                        position.y += height + EditorGUIUtility.standardVerticalSpacing;
                        totalHeight += height + EditorGUIUtility.standardVerticalSpacing;
                    }
                }
            }

            return totalHeight > 0 ? totalHeight - EditorGUIUtility.standardVerticalSpacing : 0;
        }

        /// <summary>
        /// Draws a <see cref="SerializedProperty"/> while processing all of its <see cref="InspectorDecoratorAttributeBase"/>.
        /// </summary>
        /// <param name="position">The initial GUI position.</param>
        /// <param name="serializedProperty">The property being drawn.</param>
        /// <param name="includeChildren">Should include the children when drawing?</param>
        /// <returns>The total height of the drawn GUI.</returns>
        public static float DrawSerializedProperty(Rect position, SerializedProperty serializedProperty, bool includeChildren)
        {
            using (SharedManagedPools.Pop(out List<object> scopes))
            {
                serializedProperty.GetScopes(scopes);

                InspectorCache inspectorCache = InspectorCache.Get(scopes[0].GetType());

                if (inspectorCache.Members.TryGetValue(InspectorMemberId.Get(serializedProperty), out InspectorMember inspectorMember))
                {
                    if (inspectorMember.HideInInspectorIfAttribute != null && CheckConditions(inspectorMember.HideInInspectorIfAttribute.Predicate, inspectorMember.HideInInspectorIfAttribute.Conditions))
                    {
                        return 0;
                    }

                    return inspectorMember.DrawGUI(position, scopes, serializedProperty, includeChildren);
                }

                position.height = EditorGUI.GetPropertyHeight(serializedProperty, includeChildren);
                EditorGUI.PropertyField(position, serializedProperty, includeChildren);

                return position.height;
            }
        }

        /// <summary>
        /// Gets the total height of all members that contains <see cref="ShowInInspectorAttribute"/>.
        /// </summary>
        /// <param name="scopes">The list of targets to calculate the height.</param>
        /// <param name="memberKinds">Member kinds to use to calculate the height.</param>
        /// <returns>The total height of the calculated GUI.</returns>
        public static float GetMembersWithShowInInspectorHeight(IReadOnlyList<object> scopes, ShowInInspectorMemberKinds memberKinds)
        {
            Type type = scopes[0].GetType();
            InspectorCache inspectorCache = InspectorCache.Get(type);
            float totalHeight = 0;

            if ((memberKinds & ShowInInspectorMemberKinds.Static) != 0)
            {
                for (int i = 0; i < inspectorCache.StaticMembersWithShowInInspector.Count; i++)
                {
                    InspectorMember inspectorMember = inspectorCache.StaticMembersWithShowInInspector[i];

                    if (!CheckConditions(inspectorMember.ShowInInspectorAttribute!.Predicate, inspectorMember.ShowInInspectorAttribute.Conditions))
                    {
                        continue;
                    }

                    totalHeight += inspectorMember.GetGUIHeight(scopes, null, false) + EditorGUIUtility.standardVerticalSpacing;
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

                    if (i > 0)
                    {
                        totalHeight += EditorGUIUtility.standardVerticalSpacing;
                    }

                    totalHeight += inspectorMember.GetGUIHeight(scopes, null, false) + EditorGUIUtility.standardVerticalSpacing;
                }
            }

            return totalHeight > 0 ? totalHeight - EditorGUIUtility.standardVerticalSpacing : 0;
        }

        /// <summary>
        /// Gets a <see cref="SerializedProperty"/> height considering all of its <see cref="InspectorDecoratorAttributeBase"/>.
        /// </summary>
        /// <param name="serializedProperty">The property to calculate the height.</param>
        /// <param name="includeChildren">Should include the children into the calculation?</param>
        /// <returns>The total height of the calculated GUI.</returns>
        public static float GetSerializedPropertyHeight(SerializedProperty serializedProperty, bool includeChildren)
        {
            using (SharedManagedPools.Pop(out List<object> scopes))
            {
                serializedProperty.GetScopes(scopes);

                InspectorCache inspectorCache = InspectorCache.Get(scopes[0].GetType());

                if (inspectorCache.Members.TryGetValue(InspectorMemberId.Get(serializedProperty), out InspectorMember inspectorMember))
                {
                    if (inspectorMember.HideInInspectorIfAttribute != null && CheckConditions(inspectorMember.HideInInspectorIfAttribute.Predicate, inspectorMember.HideInInspectorIfAttribute.Conditions))
                    {
                        return 0;
                    }

                    return inspectorMember.GetGUIHeight(scopes, serializedProperty, includeChildren);
                }

                return EditorGUI.GetPropertyHeight(serializedProperty, true);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float DrawCustomInspectors(Rect position, SerializedProperty iterator, bool includeChildren)
        {
            using (SharedManagedPools.Pop(out List<object> scopes))
            {
                iterator.GetScopes(scopes);

                float totalHeight = 0;
                position.height = GetMembersWithShowInInspectorHeight(scopes, ShowInInspectorMemberKinds.Static);

                if (position.height > 0)
                {
                    Vector2 offset = Vector2.right * EditorGUIUtility.standardVerticalSpacing;
                    totalHeight += EditorStyles.helpBox.CalcScreenSize(position.size).y + EditorGUIUtility.standardVerticalSpacing;

                    using (new GUI.GroupScope(position, EditorStyles.helpBox))
                    {
                        using (new EditorGUI.IndentLevelScope(-EditorGUI.indentLevel))
                        {
                            float height = DrawMembersWithShowInInspector(new Rect(offset, position.size - offset), scopes, ShowInInspectorMemberKinds.Static);

                            if (height > 0)
                            {
                                position.y += height + EditorGUIUtility.standardVerticalSpacing;
                            }
                        }
                    }
                }

                do
                {
                    bool enabled = GUI.enabled;
                    int indentLevel = EditorGUI.indentLevel;
                    float height = DrawSerializedProperty(position, iterator, includeChildren);

                    if (height > 0)
                    {
                        position.y += height + EditorGUIUtility.standardVerticalSpacing;
                        totalHeight += height + EditorGUIUtility.standardVerticalSpacing;
                    }

                    GUI.enabled = enabled;
                    EditorGUI.indentLevel = indentLevel;
                }
                while (iterator.NextVisible(false));

                if (!iterator.serializedObject.isEditingMultipleObjects)
                {
                    position.height = GetMembersWithShowInInspectorHeight(scopes, ShowInInspectorMemberKinds.Instance);

                    if (position.height > 0)
                    {
                        Vector2 offset = Vector2.right * EditorGUIUtility.standardVerticalSpacing;
                        totalHeight += EditorStyles.helpBox.CalcScreenSize(position.size).y + EditorGUIUtility.standardVerticalSpacing;

                        using (new GUI.GroupScope(position, EditorStyles.helpBox))
                        {
                            using (new EditorGUI.IndentLevelScope(-EditorGUI.indentLevel))
                            {
                                float height = DrawMembersWithShowInInspector(new Rect(offset, position.size - offset), scopes, ShowInInspectorMemberKinds.Instance);

                                if (height > 0)
                                {
                                    position.y += height + EditorGUIUtility.standardVerticalSpacing;
                                }
                            }
                        }
                    }
                }

                return totalHeight > 0 ? totalHeight - EditorGUIUtility.standardVerticalSpacing : 0;
            }
        }

        private static float GetCustomInspectorHeight(SerializedProperty iterator, bool includeChildren)
        {
            using (SharedManagedPools.Pop(out List<object> scopes))
            {
                iterator.GetScopes(scopes);

                float totalHeight = 0;
                float height = GetMembersWithShowInInspectorHeight(scopes, ShowInInspectorMemberKinds.Static);

                if (height > 0)
                {
                    totalHeight += EditorStyles.helpBox.CalcScreenSize(new Vector2(0, height)).y + EditorGUIUtility.standardVerticalSpacing;
                }

                do
                {
                    bool enabled = GUI.enabled;
                    int indentLevel = EditorGUI.indentLevel;
                    height = GetSerializedPropertyHeight(iterator, includeChildren);

                    if (height > 0)
                    {
                        totalHeight += height + EditorGUIUtility.standardVerticalSpacing;
                    }

                    GUI.enabled = enabled;
                    EditorGUI.indentLevel = indentLevel;
                }
                while (iterator.NextVisible(false));

                if (!iterator.serializedObject.isEditingMultipleObjects)
                {
                    height = GetMembersWithShowInInspectorHeight(scopes, ShowInInspectorMemberKinds.Instance);

                    if (height > 0)
                    {
                        totalHeight += EditorStyles.helpBox.CalcScreenSize(new Vector2(0, height)).y + EditorGUIUtility.standardVerticalSpacing;
                    }
                }

                return totalHeight > 0 ? totalHeight - EditorGUIUtility.standardVerticalSpacing : 0;
            }
        }
    }
}
