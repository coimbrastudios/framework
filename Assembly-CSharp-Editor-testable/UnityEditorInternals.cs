using UnityEditor;

namespace CoimbraInternal.Editor
{
    internal static class UnityEditorInternals
    {
        internal static void ClearLogEntries()
        {
            LogEntries.Clear();
        }
    }
}
