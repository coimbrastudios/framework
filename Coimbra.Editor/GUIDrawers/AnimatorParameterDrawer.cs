using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Drawer for <see cref="AnimatorParameterAttribute"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(AnimatorParameterAttribute))]
    public sealed class AnimatorParameterDrawer : ValidateDrawer
    {
        private const string NoParameterFoundError = "No {0} parameter found in Animator Controller.";

        private static readonly string[] OverrideControllerError =
        {
            "Using an Animator Override Controller is not supported.",
        };

        private static readonly string[] MultiEditErrorError =
        {
            "Cannot multi-edit different controllers.",
        };

        private static readonly List<object> Targets = new();

        private static readonly List<GUIContent> Contents = new();

        private static readonly Dictionary<AnimatorControllerParameterType, string[]> NoParameterFoundErrorDictionary = new()
        {
            [AnimatorControllerParameterType.Float] = new string[]
            {
                string.Format(NoParameterFoundError, "float"),
            },
            [AnimatorControllerParameterType.Int] = new string[]
            {
                string.Format(NoParameterFoundError, "int"),
            },
            [AnimatorControllerParameterType.Bool] = new string[]
            {
                string.Format(NoParameterFoundError, "bool"),
            },
            [AnimatorControllerParameterType.Trigger] = new string[]
            {
                string.Format(NoParameterFoundError, "trigger"),
            },
        };

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        /// <inheritdoc/>
        protected override void DrawGUI(Rect position, SerializedProperty property, GUIContent label, PropertyPathInfo context, Object[] targets, bool isDelayed)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use AnimatorParameter with string.");

                return;
            }

            AnimatorParameterAttribute animatorParameterAttribute = (AnimatorParameterAttribute)attribute;

            if (!TryGetParameters(position, label, context, targets, animatorParameterAttribute, out AnimatorControllerParameter[] parameters))
            {
                return;
            }

            PopulateContents(animatorParameterAttribute, parameters, property.stringValue, out int selectedIndex);
            DrawPopup(position, property, label, context, targets, animatorParameterAttribute, selectedIndex);
        }

        private static void DrawPopup(Rect position, SerializedProperty property, GUIContent label, PropertyPathInfo context, Object[] targets, AnimatorParameterAttribute attribute, int selectedIndex)
        {
            if (Contents.Count == 0)
            {
                ShowInvalidParameterTypePopup(position, label.text, attribute);

                return;
            }

            if (property.hasMultipleDifferentValues)
            {
                selectedIndex = -1;
            }

            static string setValue(PropertyPathInfo sender, Object target)
            {
                sender.TryGetValue(target, out string value);

                if (string.IsNullOrEmpty(value))
                {
                    return Contents[0].text;
                }

                for (int i = 0; i < Contents.Count; i++)
                {
                    if (Contents[i].text == value)
                    {
                        return value;
                    }
                }

                return Contents[0].text;
            }

            context.SetValues(targets, true, setValue);

            using EditorGUI.PropertyScope propertyScope = new(position, label, property);
            using EditorGUI.ChangeCheckScope changeCheckScope = new();
            int value = EditorGUI.Popup(position, propertyScope.content, selectedIndex, Contents.ToArray());

            if (changeCheckScope.changed)
            {
                property.stringValue = Contents[value].text;
            }
        }

        private static bool HasDifferentControllers(FieldInfo fieldInfo, IReadOnlyList<object> targets, System.Predicate<Animator> condition)
        {
            if (targets.Count <= 1)
            {
                return false;
            }

            for (int i = 1; i < targets.Count; i++)
            {
                object o = fieldInfo.GetValue(targets[i]);

                if (condition.Invoke(o as Animator))
                {
                    return true;
                }
            }

            return false;
        }

        private static void PopulateContents(AnimatorParameterAttribute attribute, IReadOnlyList<AnimatorControllerParameter> parameters, string selectedValue, out int selectedIndex)
        {
            selectedIndex = -1;
            Contents.Clear();

            for (int i = 0; i < parameters.Count; i++)
            {
                AnimatorControllerParameter parameter = parameters[i];

                if (parameter.type != attribute.ParameterType)
                {
                    continue;
                }

                if (parameter.name == selectedValue)
                {
                    selectedIndex = Contents.Count;
                }

                Contents.Add(new GUIContent(parameter.name));
            }
        }

        private static void ShowDifferentControllersPopup(Rect position, string text)
        {
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUI.Popup(position, text, 0, MultiEditErrorError);
            }
        }

        private static void ShowInvalidParameterTypePopup(Rect position, string text, AnimatorParameterAttribute attribute)
        {
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUI.Popup(position, text, 0, NoParameterFoundErrorDictionary[attribute.ParameterType]);
            }
        }

        private static bool TryGetParameters(Rect position, GUIContent label, PropertyPathInfo context, Object[] targets, AnimatorParameterAttribute attribute, [NotNullWhen(true)] out AnimatorControllerParameter[] parameters)
        {
            parameters = null;
            Targets.Clear();
            context.GetScopes(targets, Targets);

            FieldInfo animatorFieldInfo = Targets[0].GetType().FindFieldByName(attribute.AnimatorField);

            if (animatorFieldInfo == null || animatorFieldInfo.FieldType != typeof(Animator))
            {
                EditorGUI.LabelField(position, label.text, "Animator field is invalid.");

                return false;
            }

            Animator animator = (Animator)animatorFieldInfo.GetValue(Targets[0]);

            if (animator == null)
            {
                if (HasDifferentControllers(animatorFieldInfo, Targets, ObjectUtility.IsValid))
                {
                    ShowDifferentControllersPopup(position, label.text);

                    return false;
                }

                EditorGUI.LabelField(position, label.text, "Animator is null.");

                return false;
            }

            bool isNullOrDifferent(Animator otherAnimator)
            {
                return otherAnimator == null || otherAnimator.runtimeAnimatorController != animator.runtimeAnimatorController;
            }

            if (HasDifferentControllers(animatorFieldInfo, Targets, isNullOrDifferent))
            {
                ShowDifferentControllersPopup(position, label.text);

                return false;
            }

            if (animator.runtimeAnimatorController == null)
            {
                EditorGUI.LabelField(position, label.text, "Animator Controller is null.");

                return false;
            }

            AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;

            if (animatorController == null)
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUI.Popup(position, label.text, 0, OverrideControllerError);
                }

                return false;
            }

            parameters = animatorController.parameters;

            if (parameters.Length != 0)
            {
                return true;
            }

            ShowInvalidParameterTypePopup(position, label.text, attribute);

            return false;
        }
    }
}
