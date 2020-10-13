namespace Coimbra
{
    /// <summary>
    ///     General utilities.
    /// </summary>
    public static class CSUtility
    {
        /// <summary>
        /// Is currently on edit mode? Returns false outside the Unity Editor.
        /// </summary>
        public static bool IsEditMode
        {
            get
            {
#if UNITY_EDITOR
                return !UnityEditor.EditorApplication.isPlaying && !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode;
#else
                return false;
#endif
            }
        }

        /// <summary>
        /// Is currently on play mode? Returns true outside the Unity Editor.
        /// </summary>
        public static bool IsPlayMode
        {
            get
            {
#if UNITY_EDITOR
                return UnityEditor.EditorApplication.isPlaying || UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode;
#else
                return true;
#endif
            }
        }
    }
}
