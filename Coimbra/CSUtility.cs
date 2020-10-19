using UnityEditor;

namespace Coimbra
{
    internal static class CSUtility
    {
        internal static bool IsEditMode
        {
            get
            {
#if UNITY_EDITOR
                return !EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode;
#else
                return false;
#endif
            }
        }

        internal static bool IsPlayMode
        {
            get
            {
#if UNITY_EDITOR
                return EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode;
#else
                return true;
#endif
            }
        }
    }
}
