using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Window for quick access to scenes in the project.
    /// </summary>
    public sealed class SceneSelectorWindow : EditorWindow
    {
        private const string WindowsTitle = "Scene Selector";

        [SerializeField]
        private Vector2 _scrollPosition;

        [SerializeField]
        private bool _isSettingsEditorOpen;

        private UnityEditor.Editor _settingsEditor;

        private SerializedObject _serializedObject;

        private ReorderableList _reorderableList;

        /// <summary>
        /// Opens the <see cref="SceneSelectorWindow"/>.
        /// </summary>
        [MenuItem(CoimbraUtility.WindowMenuPath + WindowsTitle)]
        public static void Open()
        {
            GetWindow<SceneSelectorWindow>(WindowsTitle);
        }

        private void OnEnable()
        {
            SceneSelectorSettings settings = ScriptableSettings.Get<SceneSelectorSettings>();
            _serializedObject = new SerializedObject(settings);
            _reorderableList = new ReorderableList(settings.DisplayedScenes, typeof(SceneAsset), false, false, false, false);
            _reorderableList.drawElementCallback += DrawListElement;
            _reorderableList.drawNoneElementCallback += DrawEmptyList;
            _reorderableList.elementHeight = EditorGUIUtility.singleLineHeight;
            _settingsEditor = UnityEditor.Editor.CreateEditor(settings);
        }

        private void OnDisable()
        {
            DestroyImmediate(_settingsEditor);
            _serializedObject.Dispose();
        }

        private void DrawEmptyList(Rect rect)
        {
            EditorGUI.HelpBox(rect, "List is Empty (check Settings)", MessageType.Warning);
        }

        private void DrawListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            const float buttonWidth = 50;
            const float assetWidth = 200;
            Object sceneAsset = (Object)_reorderableList.list[index];
            string scenePath = AssetDatabase.GetAssetPath(sceneAsset);
            rect.width -= assetWidth + buttonWidth + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.SelectableLabel(rect, scenePath);

            using (new EditorGUI.DisabledScope(true))
            {
                rect.x += rect.width + EditorGUIUtility.standardVerticalSpacing;
                rect.width = assetWidth;
                EditorGUI.ObjectField(rect, GUIContent.none, sceneAsset, typeof(SceneAsset), false);
            }

            rect.x += rect.width + EditorGUIUtility.standardVerticalSpacing;
            rect.width = buttonWidth;

            if (GUI.Button(rect, "Open"))
            {
                EditorSceneManager.OpenScene(scenePath);
            }
        }

        private void OnGUI()
        {
            using EditorGUILayout.ScrollViewScope scrollView = new(_scrollPosition);
            _scrollPosition = scrollView.scrollPosition;

            SceneSelectorSettings settings = ScriptableSettings.Get<SceneSelectorSettings>();
            _isSettingsEditorOpen = EditorGUILayout.Foldout(_isSettingsEditorOpen, "Settings");

            if (_isSettingsEditorOpen)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    _settingsEditor.OnInspectorGUI();
                }
            }

            _reorderableList.list = settings.DisplayedScenes;
            _reorderableList.DoLayoutList();
        }
    }
}
