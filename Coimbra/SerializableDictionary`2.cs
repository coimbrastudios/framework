using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Coimbra
{
    /// <summary>
    /// <see cref="Dictionary{TKey,TValue}"/> that can be viewed, modified and saved from the inspector.
    /// </summary>
    /// <seealso cref="DisableResizeAttribute"/>
    /// <seealso cref="HideKeyLabelAttribute"/>
    /// <seealso cref="HideValueLabelAttribute"/>
    [Serializable]
    [CopyBaseConstructors]
    public partial class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializableDictionary
    {
        [Serializable]
        private sealed class SerializableItem
        {
            [SerializeField]
            [FormerlySerializedAs("Key")]
            private TKey _key;

            [SerializeField]
            [FormerlySerializedAs("Value")]
            private TValue _value;

            internal SerializableItem(TKey key, TValue value)
            {
                _key = key;
                _value = value;
            }

            internal TKey Key => _key;

            internal TValue Value => _value;

            public static implicit operator SerializableItem(KeyValuePair<TKey, TValue> pair)
            {
                return new SerializableItem(pair.Key, pair.Value);
            }
        }

        [SerializeField]
        [FormerlySerializedAs("_items")]
        private List<SerializableItem> _list = new();

        [SerializeField]
        [UsedImplicitly]
        private TKey _new;

        bool ISerializableDictionary.CanAdd
        {
            get
            {
                if (!typeof(TKey).IsValueType && !_new.IsValid())
                {
                    return false;
                }

                return !ContainsKey(_new);
            }
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
                if (item.Key.TryGetValid(out TKey key))
                {
                    Add(key, item.Value);
                }
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _list.Clear();

            foreach (KeyValuePair<TKey, TValue> item in this)
            {
                _list.Add(item);
            }
        }
    }
}
