using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    [CustomPropertyDrawer(typeof(DisableOnPlayModeAttribute))]
    public sealed class DisableOnPlayModeDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            return 0;
        }

        public override void OnGUI(Rect position)
        {
            GUI.enabled = CSEditorUtility.IsEditMode;
        }
    }
}
