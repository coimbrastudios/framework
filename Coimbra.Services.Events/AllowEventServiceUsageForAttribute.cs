#nullable enable

using JetBrains.Annotations;
using System;

namespace Coimbra.Services.Events
{
    /// <summary>
    /// Apply this to an <see cref="IEvent"/> to allow using <see cref="IEventService"/> directly from within the specified <see cref="Type"/>.
    /// </summary>
    /// <remarks>
    /// By default, you can never use the <see cref="IEventService"/> generic APIs directly and should use each <see cref="IEvent"/> static API instead.
    /// However, with this attribute you can have the generic APIs unblocked for specific <see cref="IService"/> implementations.
    /// </remarks>
    /// <seealso cref="IEvent"/>
    /// <seealso cref="IEventService"/>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
    [BaseTypeRequired(typeof(IEvent))]
    public sealed class AllowEventServiceUsageForAttribute : Attribute
    {
        /// <summary>
        /// If true, only the specified type will be allowed. If false, any implementation of the type will be allowed.
        /// </summary>
        public readonly bool SpecificTypeOnly;

        /// <summary>
        /// The type allowed to call the protected methods.
        /// </summary>
        public readonly Type Type;

        /// <summary>
        /// Initializes a new instance of the <see cref="AllowEventServiceUsageForAttribute"/> class.
        /// </summary>
        /// <param name="type">The type allowed to call the protected methods.</param>
        /// <param name="specificTypeOnly">If true, only the specified type will be allowed. If false, any implementation of the type will be allowed.</param>
        public AllowEventServiceUsageForAttribute(Type type, bool specificTypeOnly = false)
        {
            Type = type;
            SpecificTypeOnly = specificTypeOnly;
        }
    }
}
