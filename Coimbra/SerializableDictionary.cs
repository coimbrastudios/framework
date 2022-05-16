using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// <see cref="Dictionary{TKey,TValue}"/> that can be viewed, modified and saved from the inspector.
    /// </summary>
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializableDictionary
    {
        [Serializable]
        private sealed class SerializableItem
        {
            [SerializeField]
            internal TKey Key;

            [SerializeField]
            internal TValue Value;

            public static implicit operator SerializableItem(KeyValuePair<TKey, TValue> pair)
            {
                return new SerializableItem
                {
                    Key = pair.Key,
                    Value = pair.Value,
                };
            }
        }

        [SerializeField]
        private List<SerializableItem> _items = new List<SerializableItem>();

        [SerializeField]
        [UsedImplicitly]
        private SerializableItem _newEntry = new SerializableItem();

        public SerializableDictionary() { }

        public SerializableDictionary(IDictionary<TKey, TValue> dictionary)
            : base(dictionary) { }

        public SerializableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
            : base(dictionary, comparer) { }

        public SerializableDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer) { }

        public SerializableDictionary(int capacity)
            : base(capacity) { }

        public SerializableDictionary(int capacity, IEqualityComparer<TKey> comparer)
            : base(capacity, comparer) { }

        bool ISerializableDictionary.IsNewEntryValid => _newEntry.Key != null && !ContainsKey(_newEntry.Key);

        Type ISerializableDictionary.KeyType => typeof(TKey);

        Type ISerializableDictionary.ValueType => typeof(TValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Deserialize()
        {
            Clear();

            foreach (SerializableItem item in _items)
            {
                if (item.Key.TryGetValid(out TKey key))
                {
                    Add(key, item.Value);
                }
            }
        }

        void ISerializableDictionary.ProcessAdd()
        {
            _items.Add(_newEntry);
            Add(_newEntry.Key, _newEntry.Value);
        }

        void ISerializableDictionary.ProcessUndo()
        {
            if (_items.Count != Count)
            {
                Deserialize();
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Deserialize();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _items.Clear();

            foreach (KeyValuePair<TKey, TValue> item in this)
            {
                _items.Add(item);
            }
        }
    }
}
