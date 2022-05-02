#nullable enable

using UnityEngine;

namespace Coimbra.Inspectors.Editor
{
    /// <summary>
    /// Implement this interface to create drawers for any <see cref="InspectorDecoratorAttributeBase"/>. It is assumed that the instance can be reused.
    /// </summary>
    public interface IInspectorDecoratorDrawer
    {
        /// <summary>
        /// Implement this method to specify how tall the <see cref="OnAfterGUI"/> is in pixels.
        /// </summary>
        /// <returns>How tall the <see cref="OnAfterGUI"/> is in pixels.</returns>
        float GetAfterGUIHeight(ref InspectorDecoratorDrawerContext context);

        /// <summary>
        /// Implement this method to specify how tall the <see cref="OnBeforeGUI"/> is in pixels.
        /// </summary>
        /// <returns>How tall the <see cref="OnBeforeGUI"/> is in pixels.</returns>
        float GetBeforeGUIHeight(ref InspectorDecoratorDrawerContext context);

        /// <summary>
        /// Implement this method to make your own GUI for the drawer. This is called before any DecoratorDrawer .
        /// </summary>
        void OnAfterGUI(Rect position, ref InspectorDecoratorDrawerContext context);

        /// <summary>
        /// Implement this method to make your own GUI for the drawer. This is called after the OnGUI of the property.
        /// </summary>
        void OnBeforeGUI(Rect position, ref InspectorDecoratorDrawerContext context);
    }
}
