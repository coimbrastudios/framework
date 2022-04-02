using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Drawer for <see cref="IndentAttribute"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(IndentAttribute))]
    public sealed class IndentAttributeDrawer : DecoratorDrawer
    {
        /// <inheritdoc/>
        public override float GetHeight()
        {
            return 0;
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect position)
        {
            IndentAttribute indentAttribute = (IndentAttribute)attribute;
            EditorGUI.indentLevel += indentAttribute.Amount;
        }
    }
}
