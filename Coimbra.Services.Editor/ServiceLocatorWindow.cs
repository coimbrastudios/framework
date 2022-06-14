using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Services.Editor
{
    /// <summary>
    /// Window to check the <see cref="ServiceLocator"/> services.
    /// </summary>
    internal sealed class ServiceLocatorWindow : EditorWindow
    {
        private const string WindowsTitle = "Service Locator";

        [SerializeField]
        private Vector2 _scrollPosition;

        [SerializeField]
        internal List<ServiceWrapper> _services = new List<ServiceWrapper>();

        private SerializedObject _serializedObject;

        private SerializedProperty _servicesProperty;

        /// <summary>
        /// Opens the <see cref="ServiceLocatorWindow"/>.
        /// </summary>
        [MenuItem(CoimbraUtility.WindowMenuPath + WindowsTitle)]
        public static void Open()
        {
            GetWindow<ServiceLocatorWindow>(WindowsTitle);
        }

        private void OnEnable()
        {
            _serializedObject = new SerializedObject(this);
            _servicesProperty = _serializedObject.FindProperty(nameof(_services));
        }

        private void OnDisable()
        {
            _serializedObject.Dispose();
        }

        private void OnGUI()
        {
            using EditorGUILayout.ScrollViewScope scrollView = new EditorGUILayout.ScrollViewScope(_scrollPosition);
            _scrollPosition = scrollView.scrollPosition;

            _services.Clear();

            foreach (KeyValuePair<Type, ServiceLocator.Service> service in ServiceLocator.Services)
            {
                _services.Add(new ServiceWrapper(service.Key, service.Value));
            }

            _serializedObject.Update();

            int arraySize = _servicesProperty.arraySize;

            for (int i = 0; i < arraySize; i++)
            {
                EditorGUILayout.PropertyField(_servicesProperty.GetArrayElementAtIndex(i));
            }
        }
    }
}
