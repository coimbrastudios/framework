using System;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    ///     Indent all the following fields on the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class IndentAttribute : PropertyAttribute
    {
        public readonly int Amount;

        public IndentAttribute(int amount)
        {
            Amount = amount;
        }
    }
}
