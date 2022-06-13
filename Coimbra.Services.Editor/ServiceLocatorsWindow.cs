using Coimbra.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Services.Editor
{
    /// <summary>
    /// Window to view all created <see cref="ServiceLocator"/>.
    /// </summary>
    internal sealed class ServiceLocatorsWindow : EditorWindow
    {
        private const string WindowsTitle = "Service Locators";

        [SerializeField]
        private Vector2 _scrollPosition;

        [SerializeField]
        private SerializableDictionary<string, ServiceLocatorWrapper> _serviceLists = new SerializableDictionary<string, ServiceLocatorWrapper>();

        private SerializedObject _serializedObject;

        private SerializedProperty _serializableItems;

        /// <summary>
        /// Opens the <see cref="ServiceLocatorsWindow"/>.
        /// </summary>
        [MenuItem(CoimbraUtility.WindowMenuPath + WindowsTitle)]
        public static void Open()
        {
            GetWindow<ServiceLocatorsWindow>(WindowsTitle);
        }

        private void OnEnable()
        {
            _serializedObject = new SerializedObject(this);
            _serializableItems = _serializedObject.FindProperty(nameof(_serviceLists));
            _serializableItems = _serializableItems.FindPropertyRelative(SerializableDictionaryDrawer.ListProperty);
        }

        private void OnDisable()
        {
            _serializableItems.Dispose();
            _serializedObject.Dispose();
        }

        private void OnGUI()
        {
            using EditorGUILayout.ScrollViewScope scrollView = new EditorGUILayout.ScrollViewScope(_scrollPosition);
            _scrollPosition = scrollView.scrollPosition;

            foreach (KeyValuePair<string, WeakReference<ServiceLocator>> pair in ServiceLocator.ServiceLocators)
            {
                if (!pair.Value.TryGetTarget(out ServiceLocator serviceLocator))
                {
                    _serviceLists.Remove(pair.Key);

                    continue;
                }

                if (_serviceLists.TryGetValue(pair.Key, out ServiceLocatorWrapper wrapper))
                {
                    if (wrapper.ServiceLocatorReference != null && wrapper.ServiceLocatorReference.TryGetTarget(out ServiceLocator previous))
                    {
                        if (previous != serviceLocator)
                        {
                            List<ServiceWrapper> list = wrapper.Services;
                            wrapper = new ServiceLocatorWrapper(serviceLocator, list, pair.Value);
                            _serviceLists[pair.Key] = wrapper;
                        }
                    }
                    else
                    {
                        List<ServiceWrapper> list = wrapper.Services;
                        wrapper = new ServiceLocatorWrapper(serviceLocator, list, pair.Value);
                        _serviceLists[pair.Key] = wrapper;
                    }
                }
                else
                {
                    wrapper = new ServiceLocatorWrapper(serviceLocator, new List<ServiceWrapper>(), pair.Value);
                    _serviceLists.Add(pair.Key, wrapper);
                }

                wrapper.Services.Clear();

                foreach (KeyValuePair<Type, ServiceLocator.Service> service in serviceLocator.Services)
                {
                    wrapper.Services.Add(new ServiceWrapper(service.Key, service.Value));
                }

                _serializedObject.Update();

                int arraySize = _serializableItems.arraySize;

                for (int i = 0; i < arraySize; i++)
                {
                    EditorGUILayout.PropertyField(_serializableItems.GetArrayElementAtIndex(i).FindPropertyRelative(SerializableDictionaryDrawer.ValueProperty));
                }
            }
        }
    }
}
