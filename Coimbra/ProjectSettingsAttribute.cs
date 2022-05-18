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
        public readonly bool IsEditorOnly;

        public readonly string? NameOverride;

        public readonly string ProjectSettingsPath;

        public ProjectSettingsAttribute(bool isEditorOnly)
            : this("", null, isEditorOnly) { }

        public ProjectSettingsAttribute(string projectSettingsPath, bool isEditorOnly)
            : this(projectSettingsPath, projectSettingsPath, isEditorOnly) { }

        public ProjectSettingsAttribute(string projectSettingsPath = "", string? nameOverride = null, bool isEditorOnly = false)
        {
            ProjectSettingsPath = projectSettingsPath;
            IsEditorOnly = isEditorOnly;
            NameOverride = nameOverride;
        }

        public string EditorFileDirectory { get; set; } = "ProjectSettings";

        public string? EditorFileNameOverride { get; set; } = null;

        public string ProjectSettingsSection { get; set; } = "Project";

        public string[]? Keywords { get; set; } = null;
    }
}
