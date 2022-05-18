#nullable enable

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

        /// <param name="isEditorOnly">If true, the saved data will not be available on builds. You can still define default values for the <see cref="ScriptableSettings"/>.</param>
        public ProjectSettingsAttribute(bool isEditorOnly)
            : this(DefaultWindowPath, null, isEditorOnly) { }

        /// <param name="windowPath">The path on the project settings window.</param>
        /// <param name="isEditorOnly">If true, the saved data will not be available on builds. You can still define default values for the <see cref="ScriptableSettings"/>.</param>
        public ProjectSettingsAttribute(string windowPath, bool isEditorOnly)
            : this(windowPath, null, isEditorOnly) { }

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
        /// The directory to save the file if <see cref="IsEditorOnly"/> is true.
        /// </summary>
        public string FileDirectory { get; set; } = DefaultFileDirectory;

        /// <summary>
        /// If null, the file name will be in the format "{type}.asset".
        /// </summary>
        public string? FileNameOverride { get; set; } = null;

        /// <summary>
        /// The keywords to be used in the search functionality.
        /// </summary>
        public string[]? Keywords { get; set; } = null;
    }
}
