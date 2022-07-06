#nullable enable

using JetBrains.Annotations;
using System;

namespace Coimbra
{
    /// <summary>
    /// Apply this attribute to a <see cref="ScriptableSettings"/> that should show up in the project settings window.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    [BaseTypeRequired(typeof(ScriptableSettings))]
    public sealed class ProjectSettingsAttribute : Attribute
    {
        /// <summary>
        /// Default value for <see cref="FileDirectory"/>.
        /// </summary>
        public const string DefaultFileDirectory = "ProjectSettings";

        /// <summary>
        /// Default value for <see cref="WindowPath"/>.
        /// </summary>
        public const string DefaultWindowPath = "Project";

        /// <summary>
        /// If true, the serialized data will not be available on builds. You can still define default values for the <see cref="ScriptableSettings"/>.
        /// </summary>
        public readonly bool IsEditorOnly;

        /// <summary>
        /// If null, the name in the project settings window will be the display name of the type.
        /// </summary>
        public readonly string? NameOverride;

        /// <summary>
        /// The path on the project settings window.
        /// </summary>
        public readonly string WindowPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectSettingsAttribute"/> class.
        /// </summary>
        /// <param name="isEditorOnly">If true, the saved data will not be available on builds. You can still define default values for the <see cref="ScriptableSettings"/>.</param>
        public ProjectSettingsAttribute(bool isEditorOnly)
            : this(DefaultWindowPath, null, isEditorOnly) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectSettingsAttribute"/> class.
        /// </summary>
        /// <param name="windowPath">The path on the project settings window.</param>
        /// <param name="isEditorOnly">If true, the saved data will not be available on builds. You can still define default values for the <see cref="ScriptableSettings"/>.</param>
        public ProjectSettingsAttribute(string windowPath, bool isEditorOnly)
            : this(windowPath, null, isEditorOnly) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectSettingsAttribute"/> class.
        /// </summary>
        /// <param name="windowPath">The path on the project settings window.</param>
        /// <param name="nameOverride">If null, the name in the project settings window will be the display name of the type.</param>
        /// <param name="isEditorOnly">If true, the saved data will not be available on builds. You can still define default values for the <see cref="ScriptableSettings"/>.</param>
        public ProjectSettingsAttribute(string windowPath = DefaultWindowPath, string? nameOverride = null, bool isEditorOnly = false)
        {
            WindowPath = windowPath;
            IsEditorOnly = isEditorOnly;
            NameOverride = nameOverride;
        }

        /// <summary>
        /// Gets or sets the directory to save the file if <see cref="IsEditorOnly"/> is true.
        /// </summary>
        public string? FileDirectory { get; set; } = DefaultFileDirectory;

        /// <summary>
        /// Gets or sets a value that overrides the default file name.
        /// </summary>
        /// <value>
        /// If null, the file name will be in the format "{type}.asset".
        /// </value>
        [PublicAPI]
        public string? FileNameOverride { get; set; }

        /// <summary>
        /// Gets or sets the keywords to be used in the search functionality.
        /// </summary>
        [PublicAPI]
        public string[]? Keywords { get; set; }
    }
}
