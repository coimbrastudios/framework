using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="OnCanvasHierarchyChanged"/> callback.
    /// </summary>
    /// <seealso cref="BeforeTransformParentChangedListener"/>
    /// <seealso cref="CanvasGroupChangedListener"/>
    /// <seealso cref="RectTransformDimensionsChangeListener"/>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraListenersUtility.TransformMenuPath + "Canvas Hierarchy Changed Listener")]
    [HelpURL("https://docs.unity3d.com/Packages/com.unity.ugui@1.0/api/UnityEngine.EventSystems.UIBehaviour.html#UnityEngine_EventSystems_UIBehaviour_OnCanvasHierarchyChanged")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class CanvasHierarchyChangedListener : MonoBehaviour
    {
        public delegate void EventHandler(CanvasHierarchyChangedListener sender);

        /// <summary>
        /// Invoked inside <see cref="OnCanvasHierarchyChanged"/>.
        /// </summary>
        public event EventHandler OnTrigger;

        private void OnCanvasHierarchyChanged()
        {
            OnTrigger?.Invoke(this);
        }
    }
}
