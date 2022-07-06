using Coimbra.Editor;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Services.Editor
{
    /// <summary>
    /// Window to check the <see cref="ServiceLocator"/> services.
    /// </summary>
    [InitializeOnLoad]
    public sealed class ServiceLocatorWindow : EditorWindow
    {
        private enum WindowMode
        {
            CurrentServices = 0,

            ServicesClasses = 1,

            ServicesInterfaces = 2,
        }

        private const string WindowsTitle = "Service Locator";

        [SerializeField]
        private Vector2 _scrollPosition;

        [SerializeField]
        private WindowMode _windowMode;

        [SerializeField]
        private List<Service> _services = new();

        [SerializeField]
        private List<ServiceClass> _servicesClasses = new();

        [SerializeField]
        private List<ServiceInterface> _servicesInterfaces = new();

        private SerializedObject _serializedObject;

        private SerializedProperty _servicesClassesProperty;

        private SerializedProperty _servicesInterfacesProperty;

        private SerializedProperty _servicesProperty;

        static ServiceLocatorWindow()
        {
            EditorApplication.playModeStateChanged -= HandlePlayModeStateChanged;
            EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
        }

        /// <summary>
        /// Opens the <see cref="ServiceLocatorWindow"/>.
        /// </summary>
        [MenuItem(CoimbraUtility.WindowMenuPath + WindowsTitle)]
        public static void Open()
        {
            GetWindow<ServiceLocatorWindow>(WindowsTitle);
        }

        private static void HandlePlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            switch (playModeStateChange)
            {
                case PlayModeStateChange.EnteredEditMode:
                case PlayModeStateChange.ExitingEditMode:
                {
                    ServiceLocator.Reset();

                    break;
                }
            }
        }

        private void OnEnable()
        {
            _serializedObject = new SerializedObject(this);
            _servicesClassesProperty = _serializedObject.FindProperty(nameof(_servicesClasses));
            _servicesInterfacesProperty = _serializedObject.FindProperty(nameof(_servicesInterfaces));
            _servicesProperty = _serializedObject.FindProperty(nameof(_services));
        }

        private void OnDisable()
        {
            _services.Clear();
            _servicesClasses.Clear();
            _servicesInterfaces.Clear();
            _servicesClassesProperty.Dispose();
            _servicesInterfacesProperty.Dispose();
            _servicesProperty.Dispose();
            _serializedObject.Dispose();
        }

        private void OnGUI()
        {
            DrawToolbar();

            using EditorGUILayout.ScrollViewScope scrollView = new(_scrollPosition);
            _scrollPosition = scrollView.scrollPosition;

            using (new LabelWidthScope(EditorGUIUtility.currentViewWidth * 0.4f, LabelWidthScope.MagnitudeMode.Absolute))
            {
                switch (_windowMode)
                {
                    case WindowMode.CurrentServices:
                    {
                        DrawCurrentServices();

                        break;
                    }

                    case WindowMode.ServicesClasses:
                    {
                        DrawServiceClasses();

                        break;
                    }

                    case WindowMode.ServicesInterfaces:
                    {
                        DrawServiceInterfaces();

                        break;
                    }
                }
            }
        }

        private void DrawCurrentServices()
        {
            _services.Clear();

            foreach (KeyValuePair<Type, ServiceLocator.Service> service in ServiceLocator.Services)
            {
                _services.Add(new Service(service.Key, service.Value));
            }

            _serializedObject.Update();

            int arraySize = _servicesProperty.arraySize;

            if (arraySize == 0)
            {
                EditorGUILayout.LabelField("No service set.");

                return;
            }

            for (int i = 0; i < arraySize; i++)
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.PropertyField(_servicesProperty.GetArrayElementAtIndex(i));
                }
            }
        }

        private void DrawServiceClasses()
        {
            _servicesClasses.Clear();

            foreach (Type type in TypeCache.GetTypesDerivedFrom<IService>())
            {
                if (!type.IsInterface && type.GetCustomAttribute<HideInServiceLocatorWindowAttribute>() == null)
                {
                    _servicesClasses.Add(new ServiceClass(type));
                }
            }

            _serializedObject.Update();

            int arraySize = _servicesClassesProperty.arraySize;

            for (int i = 0; i < arraySize; i++)
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.PropertyField(_servicesClassesProperty.GetArrayElementAtIndex(i));
                }
            }
        }

        private void DrawServiceInterfaces()
        {
            _servicesInterfaces.Clear();

            foreach (Type type in TypeCache.GetTypesDerivedFrom<IService>())
            {
                if (type.IsInterface && type.GetCustomAttribute<HideInServiceLocatorWindowAttribute>() == null)
                {
                    _servicesInterfaces.Add(new ServiceInterface(type));
                }
            }

            _serializedObject.Update();

            int arraySize = _servicesInterfacesProperty.arraySize;

            for (int i = 0; i < arraySize; i++)
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.PropertyField(_servicesInterfacesProperty.GetArrayElementAtIndex(i));
                }
            }
        }

        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                using (new EditorGUI.DisabledScope(_windowMode == WindowMode.CurrentServices))
                {
                    if (GUILayout.Button("Current Services", EditorStyles.toolbarButton))
                    {
                        _windowMode = WindowMode.CurrentServices;
                    }
                }

                using (new EditorGUI.DisabledScope(_windowMode == WindowMode.ServicesClasses))
                {
                    if (GUILayout.Button("Services Classes", EditorStyles.toolbarButton))
                    {
                        _windowMode = WindowMode.ServicesClasses;
                    }
                }

                using (new EditorGUI.DisabledScope(_windowMode == WindowMode.ServicesInterfaces))
                {
                    if (GUILayout.Button("Services Interfaces", EditorStyles.toolbarButton))
                    {
                        _windowMode = WindowMode.ServicesInterfaces;
                    }
                }
            }
        }
    }
}
