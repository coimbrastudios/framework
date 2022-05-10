using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Coimbra
{
    [Serializable]
    public class SerializableMap<TKey, TValue> : Dictionary<TKey, TValue>, ISerializableMap, ISerializationCallbackReceiver
    {
        [Serializable]
        private sealed class SerializablePair
        {
            [SerializeField]
            internal TKey Key;

            [SerializeField]
            internal TValue Value;

            public static implicit operator SerializablePair(KeyValuePair<TKey, TValue> pair)
            {
                return new SerializablePair
                {
                    Key = pair.Key,
                    Value = pair.Value,
                };
            }
        }

        [SerializeField]
        private List<SerializablePair> _pairs = new();

        [SerializeField]
        [UsedImplicitly]
        private SerializablePair _pair;

        public SerializableMap() { }

        public SerializableMap(IDictionary<TKey, TValue> dictionary)
            : base(dictionary) { }

        public SerializableMap(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
            : base(dictionary, comparer) { }

        public SerializableMap(IEnumerable<KeyValuePair<TKey, TValue>> collection)
            : base(collection) { }

        public SerializableMap(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
            : base(collection, comparer) { }

        public SerializableMap(IEqualityComparer<TKey> comparer)
            : base(comparer) { }

        public SerializableMap(int capacity)
            : base(capacity) { }

        public SerializableMap(int capacity, IEqualityComparer<TKey> comparer)
            : base(capacity, comparer) { }

        bool ISerializableMap.IsPairValid => _pair.Key != null && !ContainsKey(_pair.Key);

        Type ISerializableMap.KeyType => typeof(TKey);

        Type ISerializableMap.ValueType => typeof(TValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Deserialize()
        {
            Clear();

            foreach (SerializablePair pair in _pairs)
            {
                if (pair.Key.TryGetValid(out TKey key))
                {
                    Add(key, pair.Value);
                }
            }
        }

        void ISerializableMap.ProcessAdd()
        {
            _pairs.Add(_pair);
            Add(_pair.Key, _pair.Value);
        }

        void ISerializableMap.ProcessUndo()
        {
            if (_pairs.Count != Count)
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
            _pairs.Clear();

            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                _pairs.Add(pair);
            }
        }
    }
}
