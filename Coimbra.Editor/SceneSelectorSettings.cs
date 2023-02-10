using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Coimbra.Editor
{
    [Preferences(CoimbraUtility.UserPreferencesPath, FileDirectory = CoimbraUtility.UserPreferencesFilePath)]
    internal sealed class SceneSelectorSettings : ScriptableSettings
    {
        private readonly List<SceneAsset> _displayedScenes = new();

        [SerializeField]
        [Tooltip("If enabled, all scenes added to build settings will be shown.")]
        private bool _showScenesInBuild = true;

        [SerializeField]
        [Tooltip("If enabled, will hide scenes that are inactive in the build settings. This option is ignored if 'Show Scenes in Build' is disabled.")]
        private bool _hideInactiveScenesInBuild = true;

        [SerializeField]
        [Tooltip("Any scenes that should not appear even if both added to build settings and active. This option is ignored if 'Show Scenes in Build' is disabled.")]
        private List<SceneAsset> _additionalHiddenScenes = new();

        [FormerlySerializedAs("_additionalIncludedScenes")]
        [SerializeField]
        [Tooltip("Any scenes that should appear even if not added to build settings or is inactive in the build settings. This list has higher priority than anything else and is always evaluated.")]
        private List<SceneAsset> _forceIncludedScenes = new();

        internal List<SceneAsset> DisplayedScenes
        {
            get
            {
                _displayedScenes.Clear();

                using (HashSetPool.Pop(out HashSet<string> addedSet))
                using (HashSetPool.Pop(out HashSet<string> additionalHiddenSet))
                using (HashSetPool.Pop(out HashSet<string> forceIncludedSet))
                {
                    foreach (SceneAsset sceneAsset in _additionalHiddenScenes)
                    {
                        additionalHiddenSet.Add(AssetDatabase.GetAssetPath(sceneAsset));
                    }

                    foreach (SceneAsset sceneAsset in _forceIncludedScenes)
                    {
                        forceIncludedSet.Add(AssetDatabase.GetAssetPath(sceneAsset));
                    }

                    if (_showScenesInBuild)
                    {
                        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
                        {
                            if (_hideInactiveScenesInBuild && !scene.enabled)
                            {
                                continue;
                            }

                            if (additionalHiddenSet.Contains(scene.path) && !forceIncludedSet.Contains(scene.path))
                            {
                                continue;
                            }

                            addedSet.Add(scene.path);
                            _displayedScenes.Add(AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path));
                        }
                    }

                    foreach (SceneAsset sceneAsset in _forceIncludedScenes)
                    {
                        string scenePath = AssetDatabase.GetAssetPath(sceneAsset);

                        if (addedSet.Contains(scenePath))
                        {
                            continue;
                        }

                        addedSet.Add(scenePath);
                        _displayedScenes.Add(sceneAsset);
                    }
                }

                return _displayedScenes;
            }
        }
    }
}
