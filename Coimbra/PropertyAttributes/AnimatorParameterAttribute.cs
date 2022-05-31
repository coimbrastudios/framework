using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Turns an int or string field into a popup to choose the desired animator controller parameter name.
    /// </summary>
    public sealed class AnimatorParameterAttribute : ValidateAttribute
    {
        /// <summary>
        /// The target animator field. If none, it searches for the first serialized <see cref="Animator"/> field available.
        /// </summary>
        public readonly string AnimatorField;

        /// <summary>
        /// The parameter type.
        /// </summary>
        public readonly AnimatorControllerParameterType ParameterType;

        /// <param name="parameterType">The parameter type.</param>
        public AnimatorParameterAttribute(AnimatorControllerParameterType parameterType)
        {
            AnimatorField = null;
            ParameterType = parameterType;
        }

        /// <param name="animatorField">The target animator field. If none, it searches for the first serialized <see cref="Animator"/> field available.</param>
        /// <param name="parameterType">The parameter type.</param>
        public AnimatorParameterAttribute(string animatorField, AnimatorControllerParameterType parameterType)
        {
            AnimatorField = animatorField;
            ParameterType = parameterType;
        }
    }
}
