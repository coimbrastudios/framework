#nullable enable

namespace Coimbra
{
    /// <summary>
    /// The available inspector areas to be used.
    /// </summary>
    public enum InspectorArea
    {
        /// <summary>
        /// Will fill both the label and the field area of the inspector.
        /// </summary>
        Fill = 0,

        /// <summary>
        /// Will fill the label area of the inspector.
        /// </summary>
        Label = 1,

        /// <summary>
        /// Will fill the field area of the inspector.
        /// </summary>
        Field = 2,
    }
}
