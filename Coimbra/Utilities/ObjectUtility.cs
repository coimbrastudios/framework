using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Utility methods for both Unity <see cref="Object"/> and C# <see cref="object"/> types.
    /// </summary>
    public static class ObjectUtility
    {
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
        public static bool Destroy(this Object o)
        {
            if (o is GameObject gameObject)
            {
                return gameObject.Destroy();
            }

            if (o is Actor actor)
            {
                if (actor.IsDestroyed)
                {
                    return false;
                }

                actor.Destroy();

                return true;
            }

            if (!o.TryGetValid(out o))
            {
                return false;
            }

            if (CoimbraUtility.IsPlayMode)
            {
#pragma warning disable COIMBRA0019
                Object.Destroy(o);
#pragma warning restore COIMBRA0019
            }
            else
            {
                Object.DestroyImmediate(o);
            }

            return true;
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
        public static bool TryGetValid<T>(this T o, out T valid)
        {
            valid = GetValid(o);

            return valid != null;
        }
    }
}
