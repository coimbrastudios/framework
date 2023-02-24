namespace Coimbra
{
    /// <summary>
    /// Disable the field on the inspector while on edit mode.
    /// </summary>
    /// <seealso cref="DisableAttribute"/>
    /// <seealso cref="DisableOnPlayModeAttribute"/>
    public sealed class DisableOnEditModeAttribute : DisableAttributeBase
    {
        /// <inheritdoc/>
        public override bool ShouldDisableGUI()
        {
            return ApplicationUtility.IsEditMode;
        }
    }
}
