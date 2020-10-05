using UnityEditor;
using UnityEngine;

namespace Coimbra
{
    [CustomPropertyDrawer(typeof(DisableGUIAttribute))]
    public sealed class DisableGUIDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            return 0;
        }

        public override void OnGUI(Rect position)
        {
            GUI.enabled = false;
        }
    }
}
