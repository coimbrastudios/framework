using System;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    internal sealed class TemporaryWindow : EditorWindow
    {
        private static Action<TemporaryWindow> _guiCallback;

        internal static TemporaryWindow Get(string title, Action<TemporaryWindow> guiCallback)
        {
            _guiCallback = guiCallback;

            return GetWindow<TemporaryWindow>(true, title, true);
        }

        internal static TemporaryWindow GetWithRect(Rect position, string title, Action<TemporaryWindow> guiCallback)
        {
            _guiCallback = guiCallback;

            return GetWindowWithRect<TemporaryWindow>(position, true, title, true);
        }

        private void OnGUI()
        {
            _guiCallback?.Invoke(this);
        }
    }
}
