using UnityEditor;

namespace Coimbra.Editor
{
    /// <summary>
    /// Resets the indent level to 0 temporarily.
    /// </summary>
    public sealed class ResetIndentLevelScope : EditorGUI.IndentLevelScope
    {
        public ResetIndentLevelScope()
            : base(-EditorGUI.indentLevel) { }
    }
}
