using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    [CustomPropertyDrawer(typeof(DisableOnEditModeAttribute))]
    public sealed class DisableOnEditModeDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            return 0;
        }

        public override void OnGUI(Rect position)
        {
            GUI.enabled = EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode;
        }
    }
}
