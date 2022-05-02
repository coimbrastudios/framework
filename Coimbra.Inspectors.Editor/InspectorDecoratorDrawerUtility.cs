#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Inspectors.Editor
{
    [InitializeOnLoad]
    internal static class InspectorDecoratorDrawerUtility
    {
        private static readonly Dictionary<Type, IInspectorDecoratorDrawer> DecoratorDrawerMap = new();

        static InspectorDecoratorDrawerUtility()
        {
            DecoratorDrawerMap.Clear();

            foreach (Type decoratorDrawerType in TypeCache.GetTypesDerivedFrom<IInspectorDecoratorDrawer>())
            {
                if (decoratorDrawerType.IsAbstract || !decoratorDrawerType.IsDefined(typeof(InspectorDecoratorDrawerAttribute)))
                {
                    continue;
                }

                IInspectorDecoratorDrawer decoratorDrawer = (IInspectorDecoratorDrawer)Activator.CreateInstance(decoratorDrawerType);

                foreach (InspectorDecoratorDrawerAttribute decoratorDrawerAttribute in decoratorDrawerType.GetCustomAttributes<InspectorDecoratorDrawerAttribute>())
                {
                    if (!decoratorDrawerAttribute.Type.IsSubclassOf(typeof(InspectorDecoratorAttributeBase)))
                    {
                        Debug.LogError($"{nameof(InspectorDecoratorDrawerAttribute)}.{nameof(InspectorDecoratorDrawerAttribute.Type)} expects a type that inherits from {nameof(InspectorDecoratorAttributeBase)}!");

                        continue;
                    }

                    DecoratorDrawerMap[decoratorDrawerAttribute.Type] = decoratorDrawer;

                    if (!decoratorDrawerAttribute.UseForChildren)
                    {
                        continue;
                    }

                    foreach (Type derivedType in TypeCache.GetTypesDerivedFrom(decoratorDrawerAttribute.Type))
                    {
                        if (!DecoratorDrawerMap.ContainsKey(derivedType))
                        {
                            DecoratorDrawerMap.Add(derivedType, decoratorDrawer);
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float DrawAfterGUI(Rect position, ref InspectorDecoratorDrawerContext context)
        {
            if (!DecoratorDrawerMap.TryGetValue(context.Attribute.GetType(), out IInspectorDecoratorDrawer drawer))
            {
                return 0;
            }

            position.height = drawer.GetAfterGUIHeight(ref context);
            drawer.OnAfterGUI(position, ref context);

            return position.height;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float DrawBeforeGUI(Rect position, ref InspectorDecoratorDrawerContext context)
        {
            if (!DecoratorDrawerMap.TryGetValue(context.Attribute.GetType(), out IInspectorDecoratorDrawer drawer))
            {
                return 0;
            }

            position.height = drawer.GetBeforeGUIHeight(ref context);
            drawer.OnBeforeGUI(position, ref context);

            return position.height;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float GetAfterGUIHeight(ref InspectorDecoratorDrawerContext context)
        {
            return DecoratorDrawerMap.TryGetValue(context.Attribute.GetType(), out IInspectorDecoratorDrawer drawer) ? drawer.GetAfterGUIHeight(ref context) : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float GetBeforeGUIHeight(ref InspectorDecoratorDrawerContext context)
        {
            return DecoratorDrawerMap.TryGetValue(context.Attribute.GetType(), out IInspectorDecoratorDrawer drawer) ? drawer.GetBeforeGUIHeight(ref context) : 0;
        }
    }
}
