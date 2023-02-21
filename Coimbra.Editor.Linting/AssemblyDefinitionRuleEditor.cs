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
                string label = rulesToAdd.Count > 1 ? $"Add {rulesToAdd.Count.ToString()} Rules" : "Add 1 Rule";

                if (GUILayout.Button(label))
                {
                    Undo.RecordObject(settings, label);

                    foreach (AssemblyDefinitionRuleBase rule in rulesToAdd)
                    {
                        settings.AssemblyDefinitionRules.Add(rule);
                    }

                    settings.SaveAsset();
                    LintingSettings.InitializeAssemblyDefinitionRules();
                }
            }

            using (new EditorGUI.DisabledScope(rulesToRemove.Count == 0))
            {
                string label = rulesToRemove.Count > 1 ? $"Remove {rulesToRemove.Count.ToString()} Rules" : "Remove 1 Rule";

                if (GUILayout.Button(label))
                {
                    Undo.RecordObject(settings, label);

                    foreach (AssemblyDefinitionRuleBase rule in rulesToRemove)
                    {
                        settings.AssemblyDefinitionRules.Remove(rule);
                    }

                    settings.SaveAsset();
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
                LintingSettings settings = ScriptableSettings.Get<LintingSettings>();

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
