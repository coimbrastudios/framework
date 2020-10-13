using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    [CustomPropertyDrawer(typeof(DisableAttribute), true)]
    public class DisableDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            return 0;
        }

        public override void OnGUI(Rect position)
        {
            if (attribute is DisableOnEditModeAttribute)
            {
                GUI.enabled = CSUtility.IsPlayMode;
            }
            else if (attribute is DisableOnPlayModeAttribute)
            {
                GUI.enabled = CSUtility.IsEditMode;
            }
            else
            {
                GUI.enabled = false;
            }
        }
    }
}
