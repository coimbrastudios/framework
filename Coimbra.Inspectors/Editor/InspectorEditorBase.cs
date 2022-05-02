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
        internal static Action<SerializedProperty, bool> DrawCustomInspectorsHandler = null!;

        /// <summary>
        /// If false, the disabled field showing the script will not be drawn.
        /// </summary>
        public virtual bool DrawScriptField => true;

        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            SerializedProperty iterator = serializedObject.GetIterator();

            if (iterator.NextVisible(true))
            {
                if (DrawScriptField)
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EditorGUILayout.PropertyField(iterator, true);
                    }
                }

                if (iterator.NextVisible(false))
                {
                    DrawCustomInspectors(iterator, true);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Custom each member with the extended inspector functionalities.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static void DrawCustomInspectors(SerializedProperty iterator, bool includeChildren)
        {
            DrawCustomInspectorsHandler(iterator, includeChildren);
        }
    }
}
#endif
