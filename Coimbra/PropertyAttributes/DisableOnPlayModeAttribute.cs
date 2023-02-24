namespace Coimbra
{
    /// <summary>
    /// Disable the field on the inspector while on play mode.
    /// </summary>
    /// <seealso cref="DisableAttribute"/>
    /// <seealso cref="DisableOnEditModeAttribute"/>
    public sealed class DisableOnPlayModeAttribute : DisableAttributeBase
    {
        /// <inheritdoc/>
        public override bool ShouldDisableGUI()
        {
            return ApplicationUtility.IsPlayMode;
        }
    }
}
