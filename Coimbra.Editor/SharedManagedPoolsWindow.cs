using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Window to check all <see cref="ManagedPool{T}"/> created with <see cref="SharedManagedPoolAttribute"/>.
    /// </summary>
    public sealed class SharedManagedPoolsWindow : EditorWindow
    {
        [Serializable]
        [UsedImplicitly]
        private struct ManagedPool
        {
            [SerializeField]
            [HideInInspector]
            internal string Name;

            [SerializeField]
            [Disable]
            [Tooltip("The current amount of instances available.")]
            internal int AvailableCount;

            [SerializeField]
            [Disable]
            [Tooltip("Max amount of instances in the pool. If 0 it is treated as infinity capacity.")]
            internal int MaxCapacity;

            [SerializeField]
            [Disable]
            [Tooltip("Amount of instances available from the beginning.")]
            internal int PreloadCount;

            public ManagedPool(IManagedPool managedPool)
            {
                Name = TypeString.Get(managedPool.GetType().GenericTypeArguments[0]);
                AvailableCount = managedPool.AvailableCount;
                MaxCapacity = managedPool.MaxCapacity;
                PreloadCount = managedPool.PreloadCount;
            }
        }

        private const string WindowsTitle = "Shared Managed Pools";

        [SerializeField]
        private Vector2 _scrollPosition;

        [SerializeField]
        [Disable]
        private List<ManagedPool> _managedPools = new();

        private SerializedObject _serializedObject;

        private SerializedProperty _managedPoolsProperty;

        /// <summary>
        /// Opens the <see cref="SharedManagedPoolsWindow"/>.
        /// </summary>
        [MenuItem(CoimbraUtility.WindowMenuPath + WindowsTitle)]
        public static void Open()
        {
            GetWindow<SharedManagedPoolsWindow>(WindowsTitle);
        }

        private void OnEnable()
        {
            _serializedObject = new SerializedObject(this);
            _managedPoolsProperty = _serializedObject.FindProperty(nameof(_managedPools));
        }

        private void OnDisable()
        {
            _managedPoolsProperty.Dispose();
            _serializedObject.Dispose();
        }

        private void OnGUI()
        {
            using EditorGUILayout.ScrollViewScope scrollView = new(_scrollPosition);
            _scrollPosition = scrollView.scrollPosition;
            _managedPools.Clear();

            for (int i = SharedManagedPoolUtility.All.Count - 1; i >= 0; i--)
            {
                if (SharedManagedPoolUtility.All[i].TryGetTarget(out IManagedPool value))
                {
                    _managedPools.Add(new ManagedPool(value));
                }
                else
                {
                    SharedManagedPoolUtility.All.RemoveAtSwapBack(i);
                }
            }

            _serializedObject.Update();

            int arraySize = _managedPoolsProperty.arraySize;

            for (int i = 0; i < arraySize; i++)
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.PropertyField(_managedPoolsProperty.GetArrayElementAtIndex(i));
                }
            }
        }
    }
}
