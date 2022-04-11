using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Coimbra
{
    /// <summary>
    /// Used to restrict an <see cref="AssetReference"/> field or property to only allow <see cref="GameObject"/> with a specific component. This is only enforced through the UI.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class AssetReferenceComponentRestriction : AssetReferenceUIRestriction
    {
        /// <summary>
        /// The <see cref="GameObject"/> need to have all of those components.
        /// </summary>
        public readonly Type[] All;

        /// <summary>
        /// The <see cref="GameObject"/> need to have at least one of those components.
        /// </summary>
        public readonly Type[] Any;

        /// <summary>
        /// The <see cref="GameObject"/> should not have any of those components.
        /// </summary>
        public readonly Type[] None;

        public AssetReferenceComponentRestriction(params Type[] all)
        {
            All = all;
            Any = null;
            None = null;
        }

        public AssetReferenceComponentRestriction(Type[] all, Type[] any, Type[] none)
        {
            All = all;
            Any = any;
            None = none;
        }

        public override bool ValidateAsset(string path)
        {
#if UNITY_EDITOR
            GameObject gameObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (gameObject != null)
            {
                return ValidateAsset(gameObject);
            }
#endif
            return base.ValidateAsset(path);
        }

        public override bool ValidateAsset(UnityEngine.Object obj)
        {
            if (obj is GameObject gameObject)
            {
                return ValidateAsset(gameObject);
            }

            return false;
        }

        private bool ValidateAsset(GameObject gameObject)
        {
            if (All != null)
            {
                foreach (Type type in All)
                {
                    if (!gameObject.TryGetComponent(type, out _))
                    {
                        return false;
                    }
                }
            }

            if (None != null)
            {
                foreach (Type type in None)
                {
                    if (gameObject.TryGetComponent(type, out _))
                    {
                        return false;
                    }
                }
            }

            if (Any != null)
            {
                foreach (Type type in Any)
                {
                    if (gameObject.TryGetComponent(type, out _))
                    {
                        return true;
                    }
                }

                return false;
            }

            return true;
        }
    }
}
