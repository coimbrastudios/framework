using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Coimbra
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializableMap, ISerializationCallbackReceiver
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
        private List<SerializablePair> _pairs = new List<SerializablePair>();

        [SerializeField]
        [UsedImplicitly]
        private SerializablePair _pair = new SerializablePair();

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
