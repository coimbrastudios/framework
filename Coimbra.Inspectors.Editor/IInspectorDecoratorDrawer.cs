#nullable enable

namespace Coimbra.Inspectors.Editor
{
    /// <summary>
    /// Implement this interface to create drawers for any <see cref="IInspectorDecoratorAttribute"/>. It is assumed that the instance can be reused.
    /// </summary>
    public interface IInspectorDecoratorDrawer
    {
        /// <summary>
        /// Implement this method to specify how tall the <see cref="OnAfterGUI"/> is in pixels.
        /// </summary>
        /// <returns>How tall the <see cref="OnAfterGUI"/> is in pixels.</returns>
        float GetHeightAfterGUI(ref InspectorDecoratorDrawerContext context);

        /// <summary>
        /// Implement this method to specify how tall the <see cref="OnBeforeGUI"/> is in pixels.
        /// </summary>
        /// <returns>How tall the <see cref="OnBeforeGUI"/> is in pixels.</returns>
        float GetHeightBeforeGUI(ref InspectorDecoratorDrawerContext context);

        /// <summary>
        /// Implement this method to make your own GUI for the drawer. This is called before any DecoratorDrawer .
        /// </summary>
        void OnAfterGUI(ref InspectorDecoratorDrawerContext context);

        /// <summary>
        /// Implement this method to make your own GUI for the drawer. This is called after the OnGUI of the property.
        /// </summary>
        void OnBeforeGUI(ref InspectorDecoratorDrawerContext context);
    }
}
