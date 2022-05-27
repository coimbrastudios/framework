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
    [CopyBaseConstructors]
    public partial class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializableDictionary
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

        bool ISerializableDictionary.IsNewEntryValid => _newEntry.Key != null && !ContainsKey(_newEntry.Key);

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
