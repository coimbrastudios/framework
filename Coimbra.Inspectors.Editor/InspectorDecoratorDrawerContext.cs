#nullable enable

using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Inspectors.Editor
{
    /// <summary>
    /// Current context for a <see cref="IInspectorDecoratorDrawer"/>.
    /// </summary>
    public ref struct InspectorDecoratorDrawerContext
    {
        /// <summary>
        /// The current inspector position.
        /// </summary>
        public Rect Position { get; internal set; }

        /// <summary>
        /// The label considering <see cref="LabelAttribute"/> and <see cref="TooltipAttribute"/>.
        /// </summary>
        public GUIContent Label { get; internal set; }

        /// <summary>
        /// The <see cref="MemberInfo"/> for the inspected member.
        /// </summary>
        public MemberInfo MemberInfo { get; internal set; }

        /// <summary>
        /// The decorator attribute being processed.
        /// </summary>
        public InspectorDecoratorAttributeBase Attribute { get; internal set; }

        /// <summary>
        /// All objects being inspected when editing multiple objects.
        /// </summary>
        public IReadOnlyList<Object> Scopes { get; internal set; }

        /// <summary>
        /// The <see cref="SerializedProperty"/> being inspected, if any.
        /// </summary>
        public SerializedProperty? SerializedProperty { get; internal set; }
    }
}
