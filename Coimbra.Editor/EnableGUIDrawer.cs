using UnityEditor;
using UnityEngine;

namespace Coimbra
{
    [CustomPropertyDrawer(typeof(EnableGUIAttribute))]
    public sealed class EnableGUIDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            return 0;
        }

        public override void OnGUI(Rect position)
        {
            GUI.enabled = true;
        }
    }
}
