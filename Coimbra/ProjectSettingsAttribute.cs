using JetBrains.Annotations;
using System;

namespace Coimbra
{
    /// <summary>
    /// Apply this attribute to a <see cref="ScriptableSettings"/> that should show up in the project settings window.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [BaseTypeRequired(typeof(ScriptableSettings))]
    public sealed class ProjectSettingsAttribute : Attribute
    {
        public readonly string PathOverride;

        public readonly string NameOverride;

        public ProjectSettingsAttribute(string pathOverride = null, string nameOverride = null)
        {
            PathOverride = pathOverride;
            NameOverride = nameOverride;
        }
    }
}
