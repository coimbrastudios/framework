using System;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Scope for managing the <see cref="EditorGUIUtility.labelWidth"/>.
    /// </summary>
    public sealed class LabelWidthScope : GUI.Scope
    {
        public enum MagnitudeMode
        {
            /// <summary>
            /// Will use "+=" and "-=" operators to modify the <see cref="EditorGUIUtility.labelWidth"/>. <see cref="LabelWidthScope.SavedMagnitude"/> will be the value to be used with these operators.
            /// </summary>
            Increment = 0,

            /// <summary>
            /// Will use "=" operator to modify the <see cref="EditorGUIUtility.labelWidth"/>. <see cref="LabelWidthScope.SavedMagnitude"/> will be the <see cref="EditorGUIUtility.labelWidth"/> before the scope.
            /// </summary>
            Absolute = 1,
        }

        public readonly float SavedMagnitude;

        public readonly MagnitudeMode SavedMagnitudeMode;

        public LabelWidthScope(float magnitude, MagnitudeMode magnitudeMode)
        {
            switch (SavedMagnitudeMode = magnitudeMode)
            {
                case MagnitudeMode.Increment:
                {
                    SavedMagnitude = magnitude;
                    EditorGUIUtility.labelWidth += magnitude;

                    break;
                }

                case MagnitudeMode.Absolute:
                {
                    SavedMagnitude = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = magnitude;

                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void CloseScope()
        {
            switch (SavedMagnitudeMode)
            {
                case MagnitudeMode.Increment:
                {
                    EditorGUIUtility.labelWidth -= SavedMagnitude;

                    break;
                }

                case MagnitudeMode.Absolute:
                {
                    EditorGUIUtility.labelWidth = SavedMagnitude;

                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
