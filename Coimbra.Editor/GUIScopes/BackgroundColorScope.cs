using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Scope for managing the <see cref="GUI.backgroundColor"/>.
    /// </summary>
    public sealed class BackgroundColorScope : GUI.Scope
    {
        public readonly Color SavedBackgroundColor;

        public BackgroundColorScope()
        {
            SavedBackgroundColor = GUI.backgroundColor;
        }

        public BackgroundColorScope(Color backgroundColor)
        {
            SavedBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = backgroundColor;
        }

        protected override void CloseScope()
        {
            GUI.backgroundColor = SavedBackgroundColor;
        }
    }
}
