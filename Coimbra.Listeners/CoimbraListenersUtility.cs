namespace Coimbra.Listeners
{
    /// <summary>
    /// General utilities.
    /// </summary>
    public static class CoimbraListenersUtility
    {
        /// <summary>
        /// Default menu item path for particle system listeners.
        /// </summary>
        public const string ParticleSystemMenuPath = CoimbraUtility.GeneralMenuPath + "Particle System" + MenuPathSuffix;

        /// <summary>
        /// Default menu item path for 2D physics-related listeners.
        /// </summary>
        public const string Physics2DMenuPath = CoimbraUtility.GeneralMenuPath + "Physics 2D" + MenuPathSuffix;

        /// <summary>
        /// Default menu item path for 3D physics-related listeners.
        /// </summary>
        public const string PhysicsMenuPath = CoimbraUtility.GeneralMenuPath + "Physics" + MenuPathSuffix;

        /// <summary>
        /// Default menu item path for rendering-related listeners.
        /// </summary>
        public const string RenderingMenuPath = CoimbraUtility.GeneralMenuPath + "Rendering" + MenuPathSuffix;

        /// <summary>
        /// Default menu item path for transform-related listeners.
        /// </summary>
        public const string TransformOrHierarchyMenuPath = CoimbraUtility.GeneralMenuPath + "Transform & Hierarchy" + MenuPathSuffix;

        private const string MenuPathSuffix = " Listeners/";
    }
}
