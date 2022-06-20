#nullable enable

using System;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Describes a <see cref="Delegate"/> listener.
    /// </summary>
    [Serializable]
    public struct DelegateListener
    {
        [SerializeField]
        private string _target;

        [SerializeField]
        private string _method;

        public DelegateListener(in Delegate entry)
        {
            if (entry.Target != null)
            {
                _target = entry.Target.ToString();
                _method = entry.Method.Name;
            }
            else
            {
                _target = "<null>";
                _method = $"{entry.Method.DeclaringType!.FullName}.{entry.Method.Name}";
            }
        }

        /// <summary>
        /// The target instance.
        /// </summary>
        public string Target => _target;

        /// <summary>
        /// The method to be invoked.
        /// </summary>
        public string Method => _method;

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{_target}::{_method}";
        }
    }
}
