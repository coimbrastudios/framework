using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// <see cref="Type"/> that can be viewed, modified and saved from the inspector.
    /// </summary>
    /// <typeparam name="T">Will require the type to be assignable to that.</typeparam>
    [Serializable]
    public struct SerializableType<T> : ISerializationCallbackReceiver
    {
        [SerializeField]
        private string _className;

        [SerializeField]
        private string _assemblyName;

        private Type _value;

        /// <summary>
        /// The assembly of the type.
        /// </summary>
        public string AssemblyName => _assemblyName;

        /// <summary>
        /// The name of the type.
        /// </summary>
        public string ClassName => _className;

        public SerializableType(Type type)
            : this()
        {
            Value = type;
        }

        /// <summary>
        /// Get and set the serialized type.
        /// </summary>
        public Type Value
        {
            get => _value;
            set
            {
                _value = value;

                if (_value != null && typeof(T).IsAssignableFrom(_value))
                {
                    _assemblyName = _value.Assembly.FullName;
                    _className = _value.FullName;
                }
                else
                {
                    _assemblyName = null;
                    _className = null;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Type(SerializableType<T> type)
        {
            return type.Value;
        }

        public override string ToString()
        {
            return _value == null ? "<null>" : TypeString.Get(_value);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            try
            {
                if (string.IsNullOrEmpty(_assemblyName) || string.IsNullOrEmpty(_className))
                {
                    _value = null;
                }
                else
                {
                    Assembly assembly = Assembly.Load(_assemblyName);

                    if (assembly != null)
                    {
                        _value = assembly.GetType(_className);
                    }
                }

                if (typeof(T).IsAssignableFrom(_value))
                {
                    return;
                }

                _assemblyName = null;
                _className = null;
                _value = null;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
