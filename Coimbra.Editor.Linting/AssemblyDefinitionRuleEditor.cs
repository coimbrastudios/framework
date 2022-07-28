using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor.Linting
{
    /// <summary>
    /// Base editor for any <see cref="AssemblyDefinitionRuleBase"/>.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AssemblyDefinitionRuleBase), true, isFallback = true)]
    public class AssemblyDefinitionRuleEditor : UnityEditor.Editor
    {
        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            if (serializedObject.FindProperty("_displayError").boolValue)
            {
                EditorGUILayout.HelpBox("Rule should always have at least one mask (either included or excluded).", MessageType.Error);
            }
            else
            {
                DrawToolbar();
            }

            base.OnInspectorGUI();
        }

        private static void DrawButtons(LintingSettings settings, List<AssemblyDefinitionRuleBase> rulesToAdd, List<AssemblyDefinitionRuleBase> rulesToRemove)
        {
            using (new EditorGUI.DisabledScope(rulesToAdd.Count == 0))
            {
                string label = rulesToAdd.Count > 1 ? $"Add {IntString.Get(rulesToAdd.Count)} Rules" : "Add 1 Rule";

                if (GUILayout.Button(label))
                {
                    Undo.RecordObject(settings, label);

                    foreach (AssemblyDefinitionRuleBase rule in rulesToAdd)
                    {
                        settings.AssemblyDefinitionRules.Add(rule);
                    }

                    settings.Save();
                    LintingSettings.InitializeAssemblyDefinitionRules();
                }
            }

            using (new EditorGUI.DisabledScope(rulesToRemove.Count == 0))
            {
                string label = rulesToRemove.Count > 1 ? $"Remove {IntString.Get(rulesToRemove.Count)} Rules" : "Remove 1 Rule";

                if (GUILayout.Button(label))
                {
                    Undo.RecordObject(settings, label);

                    foreach (AssemblyDefinitionRuleBase rule in rulesToRemove)
                    {
                        settings.AssemblyDefinitionRules.Remove(rule);
                    }

                    settings.Save();
                    LintingSettings.InitializeAssemblyDefinitionRules();
                }
            }
        }

        private void DrawToolbar()
        {
            using (ListPool.Pop(out List<AssemblyDefinitionRuleBase> rulesToAdd))
            using (ListPool.Pop(out List<AssemblyDefinitionRuleBase> rulesToRemove))
            using (new EditorGUILayout.HorizontalScope())
            {
                if (!ScriptableSettingsUtility.TryLoadOrCreate(out LintingSettings settings))
                {
                    Debug.LogError($"{nameof(LintingSettings)} wasn't created yet!");

                    return;
                }

                foreach (Object o in targets)
                {
                    if (!(o is AssemblyDefinitionRuleBase rule))
                    {
                        continue;
                    }

                    if (settings.AssemblyDefinitionRules.Contains(rule))
                    {
                        rulesToRemove.Add(rule);
                    }
                    else
                    {
                        rulesToAdd.Add(rule);
                    }
                }

                if (rulesToAdd.Count > 0 || rulesToRemove.Count > 0)
                {
                    DrawButtons(settings, rulesToAdd, rulesToRemove);
                }
            }
        }
    }
}
