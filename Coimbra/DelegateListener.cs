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

        [SerializeField]
        private bool _isStatic;

        public DelegateListener(in Delegate entry)
        {
            if (entry.Target != null)
            {
                _target = entry.Target.ToString();
                _method = entry.Method.Name;
                _isStatic = false;
            }
            else
            {
                _target = string.Empty;
                _method = $"{entry.Method.DeclaringType!.FullName}.{entry.Method.Name}";
                _isStatic = true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the listener static.
        /// </summary>
        public bool IsStatic => _isStatic;

        /// <summary>
        /// Gets the target instance.
        /// </summary>
        public string Target => _target;

        /// <summary>
        /// Gets the method to be invoked.
        /// </summary>
        public string Method => _method;

        /// <inheritdoc/>
        public override string ToString()
        {
            return _isStatic ? _method : $"{_target} {_method}";
        }
    }
}
