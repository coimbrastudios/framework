#if UNITY_EDITOR
#nullable enable

using System;
using System.Runtime.CompilerServices;
using UnityEditor;

namespace Coimbra.Inspectors.Editor
{
    /// <summary>
    /// Custom implementation of <see cref="Editor"/> with extended functionalities.
    /// </summary>
    public abstract class InspectorEditorBase : UnityEditor.Editor
    {
        internal static Action<InspectorEditorBase> DrawCustomInspectorHandler = null!;

        /// <summary>
        /// If false, the disabled field showing the script will not be drawn.
        /// </summary>
        public virtual bool DrawScriptField => true;

        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            DrawCustomInspector();
        }

        /// <summary>
        /// Custom implementation of <see cref="Editor.DrawDefaultInspector"/> with extended functionalities.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void DrawCustomInspector()
        {
            DrawCustomInspectorHandler(this);
        }
    }
}
#endif
