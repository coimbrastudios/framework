using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Coimbra
{
    /// <summary>
    /// Utility methods for both Unity <see cref="Object"/> and C# <see cref="object"/> types.
    /// </summary>
    public static class ObjectUtility
    {
        private static readonly string[] FindAssetsFolders = new string[]
        {
            "Assets",
            "Packages",
        };

        /// <summary>
        /// Adds an item to a <see cref="ICollection{T}"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddTo<T>(this T o, ICollection<T> collection)
        {
            collection.Add(o);
        }

        /// <summary>
        /// Destroys the <see cref="Object"/> correctly by checking if it isn't already an <see cref="Actor"/> first.
        /// </summary>
        /// <seealso cref="Actor.Dispose"/>
        public static ObjectDisposeResult Dispose(this Object o, bool forceDestroy)
        {
            if (!o.TryGetValid(out o))
            {
                return ObjectDisposeResult.None;
            }

            if (o is GameObject gameObject)
            {
                return gameObject.Dispose(forceDestroy);
            }

            if (o is Actor actor)
            {
                return actor.Dispose(forceDestroy);
            }

            if (o is IDisposable disposable)
            {
                disposable.Dispose();
            }

            if (ApplicationUtility.IsPlayMode)
            {
#pragma warning disable COIMBRA0008
                Object.Destroy(o);
#pragma warning restore COIMBRA0008
            }
            else
            {
                Object.DestroyImmediate(o);
            }

            return ObjectDisposeResult.Destroyed;
        }

        /// <summary>
        /// Finds all objects of given type in the project. If inside editor it will use AssetDatabase class, otherwise it will use <see cref="Resources.FindObjectsOfTypeAll"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Object[] FindAllAnywhere(Type type)
        {
#if UNITY_EDITOR
            if (typeof(ScriptableSettings).IsAssignableFrom(type) && ScriptableSettings.GetTypeData(type).IsEditorOnly())
            {
                string[] assets = UnityEditor.AssetDatabase.FindAssets($"t:{type.Name}", FindAssetsFolders);

                using (ListPool.Pop(out List<Object> list))
                {
                    list.EnsureCapacity(assets.Length);

                    foreach (string asset in assets)
                    {
                        Object o = UnityEditor.AssetDatabase.LoadMainAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(asset));

                        if (type.IsInstanceOfType(o))
                        {
                            list.Add(o);
                        }
                    }

                    return list.ToArray();
                }
            }
#endif
            return Resources.FindObjectsOfTypeAll(type);
        }

        /// <summary>
        /// Gets a valid object to be used with ?. and ?? operators.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetValid<T>(this T o)
        {
            if (o is Object obj)
            {
                return obj != null ? o : default;
            }

            return o;
        }

        /// <summary>
        /// Safe way to check if an object is valid even if the object is an Unity <see cref="Object"/> and got destroyed already.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(this object o)
        {
            if (o is Object obj)
            {
                return obj != null;
            }

            return o != null;
        }

        /// <summary>
        /// Safe way to check if an object is valid even if the object is an Unity <see cref="Object"/> and got destroyed already, getting a valid object to be used with ?. and ?? operators too.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetValid<T>(this T o, [NotNullWhen(true)] out T valid)
        {
            valid = GetValid(o);

            return valid != null;
        }
    }
}
