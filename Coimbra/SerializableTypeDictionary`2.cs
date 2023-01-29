using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace Coimbra
{
    /// <summary>
    /// Similar to <see cref="SerializableDictionary{TKey,TValue}"/> but the key is always a <see cref="SerializableType{T}"/> and the value accepts one <see cref="FilterTypesAttributeBase"/>.
    /// </summary>
    /// <typeparam name="TKey">The type parameter for the <see cref="SerializableType{T}"/> being used as the key.</typeparam>
    /// <typeparam name="TValue">Either <see cref="ManagedField{T}"/>, or <see cref="List{T}"/> of <see cref="ManagedField{T}"/>, or <see cref="Array"/> or <see cref="ManagedField{T}"/>.</typeparam>
    /// <typeparam name="TFilter">Additional filter to be applied to both the key and the value. Can use <see cref="FilterTypesAttributeBase"/> as the default.</typeparam>
    /// <seealso cref="DisableResizeAttribute"/>
    /// <seealso cref="HideKeyLabelAttribute"/>
    /// <seealso cref="HideValueLabelAttribute"/>
    [Serializable]
    [CopyBaseConstructors]
    public partial class SerializableTypeDictionary<TKey, TValue, TFilter> : Dictionary<SerializableType<TKey>, TValue>, ISerializableDictionary
        where TFilter : FilterTypesAttributeBase
    {
        [Serializable]
        private sealed class SerializableItem
        {
            [SerializeField]
            [FormerlySerializedAs("Key")]
            [FilterTypesByMethod(nameof(FilterKeyTypes))]
            private SerializableType<TKey> _key;

            [SerializeField]
            [FormerlySerializedAs("Value")]
            [FilterTypesByMethod(nameof(FilterValueTypes))]
            private TValue _value;

            internal SerializableItem(SerializableType<TKey> key, TValue value)
            {
                _key = key;
                _value = value;
            }

            internal SerializableType<TKey> Key => _key;

            internal TValue Value => _value;

            public static implicit operator SerializableItem(KeyValuePair<SerializableType<TKey>, TValue> pair)
            {
                return new SerializableItem(pair.Key, pair.Value);
            }

            internal static bool FilterKeyTypes(PropertyPathInfo context, Object target, Type type)
            {
                Targets[0] = target;

                return !typeof(TFilter).TryCreateInstance(out FilterTypesAttributeBase filterTypesAttributeBase) || filterTypesAttributeBase.Validate(context, Targets, type);
            }

            private bool FilterValueTypes(PropertyPathInfo context, Object target, Type type)
            {
                return _key.Value.IsAssignableFrom(type) && FilterKeyTypes(context, target, type);
            }
        }

        private static readonly Object[] Targets = new Object[1];

        [SerializeField]
        [FormerlySerializedAs("_items")]
        private List<SerializableItem> _list = new();

        [SerializeField]
        [UsedImplicitly]
        [FilterTypesByMethod(nameof(FilterNewKeyTypes))]
        private SerializableType<TKey> _new;

        bool ISerializableDictionary.CanAdd => !ContainsKey(_new);

        private static bool FilterNewKeyTypes(PropertyPathInfo context, Object target, Type type)
        {
            return SerializableItem.FilterKeyTypes(context, target, type);
        }

        void ISerializableDictionary.Add()
        {
            SerializableItem item = new(_new, default);
            _list.Add(item);
            Add(item.Key, item.Value);

            _new = default;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Clear();

            foreach (SerializableItem item in _list)
            {
                if (item.Key.TryGetValid(out SerializableType<TKey> key))
                {
                    Add(key, item.Value);
                }
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _list.Clear();

            foreach (KeyValuePair<SerializableType<TKey>, TValue> item in this)
            {
                _list.Add(item);
            }
        }
    }
}
