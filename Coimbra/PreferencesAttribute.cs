#nullable enable

using JetBrains.Annotations;
using System;

namespace Coimbra
{
    /// <summary>
    /// Apply this attribute to a <see cref="ScriptableSettings"/> that should show up in the preferences window.
    /// <para>A <see cref="ScriptableSettings"/> with this attribute will not have its serialized data available on builds, but you can still define default values for it.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    [BaseTypeRequired(typeof(ScriptableSettings))]
    public sealed class PreferencesAttribute : Attribute
    {
        /// <summary>
        /// Default value for <see cref="FileDirectory"/>.
        /// </summary>
        public const string DefaultFileDirectory = "UserSettings";

        /// <summary>
        /// Default value for <see cref="WindowPath"/>.
        /// </summary>
        public const string DefaultWindowPath = "Preferences";

        /// <summary>
        /// If true, the data will be shared between all instances of the editor for the current user. The key will be the defined file name.
        /// <seealso cref="FileNameOverride"/>
        /// </summary>
        public readonly bool UseEditorPrefs;

        /// <summary>
        /// If null, the name in the preferences window will be the display name of the type.
        /// </summary>
        public readonly string? NameOverride;

        /// <summary>
        /// The path on the preferences window.
        /// </summary>
        public readonly string WindowPath;

        /// <param name="useEditorPrefs">If true, the data will be shared between all instances of the editor for the current user. The key will be the defined file name. <seealso cref="FileNameOverride"/></param>
        public PreferencesAttribute(bool useEditorPrefs)
            : this(DefaultWindowPath, null, useEditorPrefs) { }

        /// <param name="windowPath">The path on the preferences window.</param>
        /// <param name="useEditorPrefs">If true, the data will be shared between all instances of the editor for the current user. The key will be the defined file name. <seealso cref="FileNameOverride"/></param>
        public PreferencesAttribute(string windowPath, bool useEditorPrefs)
            : this(windowPath, null, useEditorPrefs) { }

        /// <param name="windowPath">The path on the preferences window.</param>
        /// <param name="nameOverride">If null, the name in the preferences window will be the display name of the type.</param>
        /// <param name="useEditorPrefs">If true, the data will be shared between all instances of the editor for the current user. The key will be the defined file name. <seealso cref="FileNameOverride"/></param>
        public PreferencesAttribute(string windowPath = DefaultWindowPath, string? nameOverride = null, bool useEditorPrefs = false)
        {
            WindowPath = windowPath;
            UseEditorPrefs = useEditorPrefs;
            NameOverride = nameOverride;
        }

        /// <summary>
        /// The directory to save the file if <see cref="UseEditorPrefs"/> is false.
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
