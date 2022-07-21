#nullable enable

using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Utility methods for <see cref="Actor"/>.
    /// </summary>
    public static class ActorUtility
    {
        /// <summary>
        /// Create a new actor of the given type.
        /// </summary>
        public static T Create<T>(in string? name = null, params SerializableType<ActorComponentBase>[] components)
            where T : Actor
        {
            T actor = new GameObject(name ?? typeof(T).Name).AddComponent<T>()!;

            foreach (SerializableType<ActorComponentBase> component in components)
            {
                if (component.Value != typeof(ActorComponentBase))
                {
                    actor.GameObject.AddComponent(component);
                }
            }

            return actor;
        }

        /// <summary>
        /// Create a new <see cref="Actor"/>.
        /// </summary>
        [MenuItem(CoimbraUtility.GameObjectMenuName + "Actor")]
        public static void CreateActor(MenuCommand command)
        {
            if (command.context is GameObject gameObject)
            {
                Create<Actor>().Transform.SetParent(gameObject.transform);
            }
            else
            {
                Create<Actor>();
            }
        }

        /// <summary>
        /// Create a new <see cref="Actor"/> with <see cref="DebugOnly"/>.
        /// </summary>
        [MenuItem(CoimbraUtility.GameObjectMenuName + "Actor (Debug Only)")]
        public static void CreateDebugOnlyActor(MenuCommand command)
        {
            const string name = "Actor (Debug Only)";

            if (command.context is GameObject gameObject)
            {
                Create<Actor>(name, typeof(DebugOnly)).Transform.SetParent(gameObject.transform);
            }
            else
            {
                Create<Actor>(name, typeof(DebugOnly));
            }
        }

        /// <summary>
        /// Create a new <see cref="GameObjectPool"/>.
        /// </summary>
        [MenuItem(CoimbraUtility.GameObjectMenuName + "GameObject Pool")]
        public static void CreateGameObjectPool(MenuCommand command)
        {
            if (command.context is GameObject gameObject)
            {
                Create<GameObjectPool>().Transform.SetParent(gameObject.transform);
            }
            else
            {
                Create<GameObjectPool>();
            }
        }

        /// <summary>
        /// Create a new <see cref="HierarchyFolder"/>.
        /// </summary>
        [MenuItem(CoimbraUtility.GameObjectMenuName + "Hierarchy Folder")]
        public static void CreateHierarchyFolder(MenuCommand command)
        {
            if (command.context is GameObject gameObject)
            {
                Create<HierarchyFolder>().Transform.SetParent(gameObject.transform);
            }
            else
            {
                Create<HierarchyFolder>();
            }
        }
    }
}
