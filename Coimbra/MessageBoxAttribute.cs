using System;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    ///     Display a message in the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class MessageBoxAttribute : PropertyAttribute
    {
        public readonly string Message;

        public readonly MessageBoxType Type;

        public MessageBoxAttribute(string message, MessageBoxType type = MessageBoxType.None)
        {
            Message = message;
            Type = type;
        }
    }
}
