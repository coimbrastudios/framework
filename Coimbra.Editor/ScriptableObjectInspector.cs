using UnityEditor;
using UnityEngine;

namespace Coimbra
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ScriptableObject), true)]
    internal sealed class ScriptableObjectInspector : ObjectInspectorBase { }
}
