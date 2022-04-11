using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Drawer for <see cref="DisableAttribute"/>, <see cref="DisableOnEditModeAttribute"/>, and <see cref="DisableOnPlayModeAttribute"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(DisableAttribute))]
    [CustomPropertyDrawer(typeof(DisableOnEditModeAttribute))]
    [CustomPropertyDrawer(typeof(DisableOnPlayModeAttribute))]
    public sealed class DisableDrawer : DecoratorDrawer
    {
        /// <inheritdoc/>
        public override float GetHeight()
        {
            return 0;
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect position)
        {
            switch (attribute)
            {
                case DisableOnEditModeAttribute _:
                {
                    if (CoimbraUtility.IsEditMode)
                    {
                        GUI.enabled = false;
                    }

                    break;
                }

                case DisableOnPlayModeAttribute _:
                {
                    if (CoimbraUtility.IsPlayMode)
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
