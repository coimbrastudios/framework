using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    [CustomPropertyDrawer(typeof(DisableAttribute))]
    [CustomPropertyDrawer(typeof(DisableOnEditModeAttribute))]
    [CustomPropertyDrawer(typeof(DisableOnPlayModeAttribute))]
    public sealed class DisableDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            return 0;
        }

        public override void OnGUI(Rect position)
        {
            switch (attribute)
            {
                case DisableOnEditModeAttribute _:
                {
                    if (FrameworkUtility.IsEditMode)
                    {
                        GUI.enabled = false;
                    }

                    break;
                }

                case DisableOnPlayModeAttribute _:
                {
                    if (FrameworkUtility.IsPlayMode)
                    {
                        GUI.enabled = false;
                    }

                    break;
                }

                default:
                {
                    GUI.enabled = false;

                    break;
                }
            }
        }
    }
}
