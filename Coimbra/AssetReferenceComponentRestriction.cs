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

        public AssetReferenceComponentRestriction(params Type[] all)
        {
            All = all;
            Any = null;
            None = null;
        }

        /// <summary>
        /// Gets or sets the components that <see cref="GameObject"/> should have at least one.
        /// </summary>
        public Type[] Any { get; set; }

        /// <summary>
        /// Gets or sets the components that <see cref="GameObject"/> should not have.
        /// </summary>
        public Type[] None { get; set; }

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

        private bool ValidateComponentsInAll(GameObject gameObject)
        {
            if (All == null)
            {
                return true;
            }

            foreach (Type type in All)
            {
                if (!gameObject.TryGetComponent(type, out _))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ValidateComponentsInNone(GameObject gameObject)
        {
            if (None == null)
            {
                return true;
            }

            foreach (Type type in None)
            {
                if (gameObject.TryGetComponent(type, out _))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ValidateAsset(GameObject gameObject)
        {
            if (!ValidateComponentsInAll(gameObject))
            {
                return false;
            }

            if (!ValidateComponentsInNone(gameObject))
            {
                return false;
            }

            if (Any == null || Any.Length == 0)
            {
                return true;
            }

            foreach (Type type in Any)
            {
                if (gameObject.TryGetComponent(type, out _))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
