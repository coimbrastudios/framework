using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Coimbra.Editor
{
    /// <summary>
    /// Drawer for <see cref="EnumFlagsAttribute"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public sealed class EnumFlagsDrawer : ValidateDrawer
    {
        private static readonly Dictionary<Type, int> Offset = new();

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        /// <inheritdoc/>
        protected override void DrawGUI(Rect position, SerializedProperty property, GUIContent label, PropertyPathInfo context, Object[] targets, bool isDelayed)
        {
            if (property.propertyType != SerializedPropertyType.Enum || !context.PropertyType.IsDefined(typeof(FlagsAttribute), false))
            {
                EditorGUI.LabelField(position, label.text, "Use EnumFlags with flags Enum.");

                return;
            }

            using (new ShowMixedValueScope())
            using (ListPool.Pop(out List<Enum> values))
            {
                context.GetValues(targets, values);

                Enum value = values[0];

                for (int i = 1; i < values.Count; i++)
                {
                    if (value.Equals(values[i]))
                    {
                        continue;
                    }

                    EditorGUI.showMixedValue = true;

                    break;
                }

                using EditorGUI.PropertyScope propertyScope = new(position, label, property);
                using EditorGUI.ChangeCheckScope changeCheckScope = new();
                value = EditorGUI.EnumFlagsField(position, propertyScope.content, value);

                if (!changeCheckScope.changed)
                {
                    return;
                }

                int valueIndex = Convert.ToInt32(value);

                if (valueIndex < -1)
                {
                    if (!Offset.TryGetValue(context.PropertyType, out int offset))
                    {
                        offset = Enum.GetValues(context.PropertyType).Cast<int>().Max() * 2;
                        Offset.Add(context.PropertyType, offset);
                    }

                    valueIndex += offset;
                }

                property.intValue = valueIndex;
            }
        }
    }
}
