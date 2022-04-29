#nullable enable

using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Coimbra.Inspectors.Editor
{
    internal sealed class InspectorMember
    {
        internal readonly MemberInfo MemberInfo;

        internal readonly GUIContent Label;

        internal readonly ShowInInspectorAttribute? ShowInInspectorAttribute;

        internal readonly HideInInspectorIfAttribute? HideInInspectorIfAttribute;

        internal readonly IReadOnlyList<InspectorDecoratorAttributeBase> DecoratorAttributes;

        internal InspectorMember(MemberInfo memberInfo,
                                 GUIContent label,
                                 ShowInInspectorAttribute? showInInspectorAttribute,
                                 HideInInspectorIfAttribute? hideInInspectorIfAttribute,
                                 IReadOnlyList<InspectorDecoratorAttributeBase> decoratorAttributes)

        {
            MemberInfo = memberInfo;
            Label = label;
            ShowInInspectorAttribute = showInInspectorAttribute;
            HideInInspectorIfAttribute = hideInInspectorIfAttribute;
            DecoratorAttributes = decoratorAttributes;
        }
    }
}
