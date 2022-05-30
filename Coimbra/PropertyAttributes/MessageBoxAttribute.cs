using System;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Display a message in the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class MessageBoxAttribute : PropertyAttribute
    {
        public readonly string Message;

        public readonly MessageBoxType Type;

        public readonly InspectorArea Area;

        public MessageBoxAttribute(string message, InspectorArea area)
            : this(message, MessageBoxType.None, area) { }

        public MessageBoxAttribute(string message, MessageBoxType type = MessageBoxType.None, InspectorArea area = InspectorArea.Fill)
        {
            Message = message;
            Type = type;
            Area = area;
        }
    }
}
