using UnityEditor;
using UnityEngine;

namespace Coimbra
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MonoBehaviour), true)]
    internal sealed class MonoBehaviourInspector : ObjectInspectorBase { }
}
