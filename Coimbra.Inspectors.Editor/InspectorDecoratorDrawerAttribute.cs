#nullable enable

using JetBrains.Annotations;
using System;

namespace Coimbra.Inspectors.Editor
{
    /// <summary>
    /// Add to a class that implements <see cref="IInspectorDecoratorDrawer"/> to assign which attribute it is responsible for.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    [BaseTypeRequired(typeof(IInspectorDecoratorDrawer))]
    public sealed class InspectorDecoratorDrawerAttribute : Attribute
    {
        public readonly bool UseForChildren;

        public readonly Type Type;

        public InspectorDecoratorDrawerAttribute(Type type, bool useForChildren = false)
        {
            Type = type;
            UseForChildren = useForChildren;
        }
    }
}
