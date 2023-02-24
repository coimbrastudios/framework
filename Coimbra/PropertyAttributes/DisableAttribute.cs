namespace Coimbra
{
    /// <summary>
    /// Always disable the field on the inspector.
    /// </summary>
    /// <seealso cref="DisableOnEditModeAttribute"/>
    /// <seealso cref="DisableOnPlayModeAttribute"/>
    public sealed class DisableAttribute : DisableAttributeBase
    {
        /// <inheritdoc/>
        public override bool ShouldDisableGUI()
        {
            return true;
        }
    }
}
