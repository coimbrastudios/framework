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

        /// <summary>
        /// The value before entering this scope.
        /// </summary>
        public readonly float SavedMagnitude;

        /// <summary>
        /// The <see cref="MagnitudeMode"/> of this scope.
        /// </summary>
        public readonly MagnitudeMode CurrentMagnitudeMode;

        public LabelWidthScope(float magnitude, MagnitudeMode magnitudeMode)
        {
            switch (CurrentMagnitudeMode = magnitudeMode)
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
            switch (CurrentMagnitudeMode)
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
