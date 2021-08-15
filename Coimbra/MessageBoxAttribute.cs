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
        public readonly bool FillLabelArea;

        public readonly string Message;

        public readonly MessageBoxType Type;

        public MessageBoxAttribute(string message, MessageBoxType type)
            : this(message, true, type) { }

        public MessageBoxAttribute(string message, bool fillLabelArea = true, MessageBoxType type = MessageBoxType.None)
        {
            FillLabelArea = fillLabelArea;
            Message = message;
            Type = type;
        }
    }
}
