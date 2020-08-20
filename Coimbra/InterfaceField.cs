using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Coimbra
{
    [Serializable]
#if !UNITY_2020_1_OR_NEWER
    public class InterfaceField<T>
#else
    public struct InterfaceField<T>
#endif
        where T : class
    {
        [SerializeReference] private T _systemObject;
        [SerializeField] private Object _unityObject;

        private T _value;

#if !UNITY_2020_1_OR_NEWER
        public InterfaceField()
        {
            _systemObject = null;
            _unityObject = null;
        }
#endif
        public InterfaceField(T value)
            : this()
        {
            Value = value;
        }

        public T Value
        {
            get => _value;
            set
            {
                _value = value;

                if (_value is Object o)
                {
                    _unityObject = o;
                    _systemObject = null;
                }
                else
                {
                    _systemObject = value;
                    _unityObject = null;
                }
            }
        }

        public static implicit operator T(InterfaceField<T> target)
        {
            return target.Value;
        }

        public static implicit operator InterfaceField<T>(T target)
        {
            return new InterfaceField<T>(target);
        }
    }
}
