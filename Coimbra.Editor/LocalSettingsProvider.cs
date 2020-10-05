using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.SettingsManagement;
using UnityEngine;

namespace Coimbra
{
    internal sealed class LocalSettingsProvider
    {
        [UserSetting] private static readonly LocalSetting<bool> EnableExtendedInspectorGloballySetting = new LocalSetting<bool>(FrameworkUtility.PackageName + ".localBuilds.enableExtendedInspectorGlobally", true);

        private static readonly GUIContent EnableExtendedInspectorGloballyLabel = new GUIContent("Enable Extended Inspector Globally*",
                                                                                                 "If false, the ExtendedInspector attribute will need to be added to each class that should be drawn with the extended inspector functionalities.");
        private static Settings _settings;

        internal static bool EnableExtendedInspectorGlobally => EnableExtendedInspectorGloballySetting.value;
        [NotNull]
        internal static Settings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new Settings(new[]
                    {
                        new UserSettingsFolderRepository(FrameworkUtility.PackageName, "Settings"),
                    });
                }

                return _settings;
            }
        }

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider()
        {
            return new UserSettingsProvider(FrameworkUtility.UserPreferencesPath, Settings, new[]
            {
                typeof(LocalSettingsProvider).Assembly,
            });
        }

        [UsedImplicitly] [UserSettingBlock(" ")]
        private static void DrawHeaderBlock(string searchContext)
        {
            using (EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                EnableExtendedInspectorGloballySetting.value = SettingsGUILayout.SettingsToggle(EnableExtendedInspectorGloballyLabel, EnableExtendedInspectorGloballySetting, searchContext);

                if (changeCheckScope.changed)
                {
                    Settings.Save();
                }
            }
        }
    }
}
