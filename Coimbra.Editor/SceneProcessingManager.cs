using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Coimbra.Editor
{
    internal class SceneProcessingManager : IProcessSceneWithReport
    {
        private static Scene _current;

        private static SceneProcessingSettings Settings => ScriptableSettingsUtility.LoadOrCreate<SceneProcessingSettings>()!;

        int IOrderedCallback.callbackOrder => Settings.ProcessSceneCallbackOrder;

        [PostProcessScene]
        private static void HandlePostProcessScene()
        {
            if (Settings.DisableScenePostProcessorComponentCallback)
            {
                _current = default;

                return;
            }

            foreach (GameObject root in _current.GetRootGameObjects())
            {
                using (ListPool.Pop(out List<IScenePostProcessorComponent> components))
                {
                    root.GetComponentsInChildren(components);

                    foreach (IScenePostProcessorComponent component in components)
                    {
                        component.OnPostProcessScene();
                    }
                }
            }

            _current = default;
        }

        void IProcessSceneWithReport.OnProcessScene(Scene scene, BuildReport report)
        {
            _current = scene;

            if (Settings.DisableSceneProcessorComponentCallback)
            {
                return;
            }

            foreach (GameObject root in scene.GetRootGameObjects())
            {
                using (ListPool.Pop(out List<ISceneProcessorComponent> components))
                {
                    root.GetComponentsInChildren(components);

                    foreach (ISceneProcessorComponent component in components)
                    {
                        component.OnProcessScene();
                    }
                }
            }
        }
    }
}
