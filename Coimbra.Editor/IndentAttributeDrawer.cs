using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    [CustomPropertyDrawer(typeof(IndentAttribute))]
    public sealed class IndentAttributeDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            return 0;
        }

        public override void OnGUI(Rect position)
        {
            IndentAttribute indentAttribute = (IndentAttribute)attribute;
            EditorGUI.indentLevel += indentAttribute.Amount;
        }
    }
}
