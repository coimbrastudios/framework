using UnityEditor;

namespace Coimbra.Editor
{
    internal static class PoolingSettingsProviderRegister
    {
        [SettingsProvider]
        private static SettingsProvider CreatePoolingSettingsProvider()
        {
            return new ScriptableSettingsProvider($"{CoimbraUtility.ProjectSettingsPath}/{CoimbraEditorGUIUtility.ToDisplayName(nameof(PoolingSettings))}", typeof(PoolingSettings));
        }
    }
}
