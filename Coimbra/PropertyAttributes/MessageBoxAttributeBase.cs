using System;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Base class to display a message in the inspector.
    /// </summary>
    /// <seealso cref="MessageBoxAttribute"/>
    /// <seealso cref="MessageBoxOnEditModeAttribute"/>
    /// <seealso cref="MessageBoxOnPlayModeAttribute"/>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public abstract class MessageBoxAttributeBase : PropertyAttribute
    {
        public readonly string Message;

        public readonly MessageBoxType Type;

        public readonly InspectorArea Area;

        protected MessageBoxAttributeBase(string message, InspectorArea area)
            : this(message, MessageBoxType.None, area) { }

        protected MessageBoxAttributeBase(string message, MessageBoxType type = MessageBoxType.None, InspectorArea area = InspectorArea.Fill)
        {
            Message = message;
            Type = type;
            Area = area;
        }

        public abstract bool ShouldDisplayMessageBox();
    }
}
