#nullable enable

using Coimbra.Editor;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Coimbra.Inspectors.Editor
{
    internal sealed class InspectorCache
    {
        internal readonly IReadOnlyList<InspectorMember> InstanceMembersWithShowInInspector;

        internal readonly IReadOnlyList<InspectorMember> StaticMembersWithShowInInspector;

        internal readonly IReadOnlyDictionary<InspectorMemberId, InspectorMember> Members;

        private static readonly Dictionary<Type, InspectorCache> CacheMap = new();

        private InspectorCache(Type type)
        {
            IReadOnlyList<MemberInfo> staticMemberInfos = type.GetInspectorMembers(BindingFlags.Static);
            IReadOnlyList<MemberInfo> instanceMemberInfos = type.GetInspectorMembers(BindingFlags.Instance);
            List<InspectorMember> staticMembersWithShowInInspector = new(staticMemberInfos.Count);
            List<InspectorMember> instanceMembersWithShowInInspector = new(instanceMemberInfos.Count);
            Dictionary<InspectorMemberId, InspectorMember> members = new(staticMemberInfos.Count + instanceMemberInfos.Count);

            for (int i = 0; i < staticMemberInfos.Count; i++)
            {
                MemberInfo memberInfo = staticMemberInfos[i];
                ShowInInspectorAttribute showInInspectorAttribute = memberInfo.GetCustomAttribute<ShowInInspectorAttribute>();

                if (showInInspectorAttribute == null)
                {
                    continue;
                }

                LabelAttribute labelAttribute = memberInfo.GetCustomAttribute<LabelAttribute>();
                List<InspectorDecoratorAttributeBase> decoratorAttribute = new(memberInfo.GetCustomAttributes<InspectorDecoratorAttributeBase>());
                decoratorAttribute.Sort(SortableComparer.Default);

                GUIContent label = new(labelAttribute?.Label ?? CoimbraEditorGUIUtility.ToDisplayName(memberInfo.Name));
                InspectorMember member = new(memberInfo, label, showInInspectorAttribute, null, decoratorAttribute);
                staticMembersWithShowInInspector.Add(member);
            }

            for (int i = 0; i < instanceMemberInfos.Count; i++)
            {
                MemberInfo memberInfo = instanceMemberInfos[i];
                LabelAttribute? labelAttribute = memberInfo.GetCustomAttribute<LabelAttribute>();
                ShowInInspectorAttribute? showInInspectorAttribute = memberInfo.GetCustomAttribute<ShowInInspectorAttribute>();
                HideInInspectorIfAttribute? hideInInspectorIfAttribute = memberInfo.GetCustomAttribute<HideInInspectorIfAttribute>();

                if (labelAttribute == null
                 && showInInspectorAttribute == null
                 && hideInInspectorIfAttribute == null
                 && !memberInfo.IsDefined(typeof(InspectorDecoratorAttributeBase), true))
                {
                    continue;
                }

                List<InspectorDecoratorAttributeBase> decoratorAttribute = new(memberInfo.GetCustomAttributes<InspectorDecoratorAttributeBase>());
                decoratorAttribute.Sort(SortableComparer.Default);

                GUIContent label = new(labelAttribute?.Label ?? CoimbraEditorGUIUtility.ToDisplayName(memberInfo.Name))
                {
                    tooltip = memberInfo.GetCustomAttribute<TooltipAttribute>()?.tooltip ?? null,
                };

                InspectorMember member = new(memberInfo, label, showInInspectorAttribute, hideInInspectorIfAttribute, decoratorAttribute);

                if (showInInspectorAttribute != null)
                {
                    instanceMembersWithShowInInspector.Add(member);
                }
            }

            staticMembersWithShowInInspector.Capacity = staticMembersWithShowInInspector.Count;
            instanceMembersWithShowInInspector.Capacity = instanceMembersWithShowInInspector.Count;
            members.TrimExcess();

            StaticMembersWithShowInInspector = staticMembersWithShowInInspector;
            InstanceMembersWithShowInInspector = instanceMembersWithShowInInspector;
            Members = members;
        }

        internal static InspectorCache Get(Type type)
        {
            if (CacheMap.TryGetValue(type, out InspectorCache value))
            {
                return value;
            }

            value = new InspectorCache(type);
            CacheMap.Add(type, value);

            return value;
        }
    }
}
