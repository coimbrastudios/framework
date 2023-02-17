using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    [Preferences(null, false)]
    [ScriptableSettingsProvider(typeof(LoadOrCreateScriptableSettingsProvider))]
    internal sealed class EditorStartupSceneStateSettings : ScriptableSettings
    {
        [field: SerializeField]
        public bool HasSavedStartupScene { get; set; }

        [field: SerializeField]
        public SceneAsset SavedStartupScene { get; set; }
    }
}
