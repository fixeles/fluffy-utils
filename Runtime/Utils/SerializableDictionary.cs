using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    /// <summary>
    /// Generic Serializable Dictionary for Unity 2020.1 and above.
    /// Simply declare your key/value types and you're good to go - zero boilerplate.
    /// </summary>
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        // Internal
        [SerializeField] private List<KeyValuePair> list = new();
        private Dictionary<TKey, int> _indexByKey = new();
        private Dictionary<TKey, TValue> _dict = new();

#pragma warning disable 0414
        [SerializeField, HideInInspector] private bool keyCollision;
#pragma warning restore 0414

        [Serializable]
        private struct KeyValuePair
        {
            public TKey Key;
            public TValue Value;

            public KeyValuePair(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }

        // Lists are serialized natively by Unity, no custom implementation needed.
        public void OnBeforeSerialize()
        {
        }

        // Populate dictionary with pairs from list and flag key-collisions.
        public void OnAfterDeserialize()
        {
            _dict.Clear();
            _indexByKey.Clear();
            keyCollision = false;
            for (int i = 0; i < list.Count; i++)
            {
                var key = list[i].Key;
                if (key != null && !ContainsKey(key))
                {
                    _dict.Add(key, list[i].Value);
                    _indexByKey.Add(key, i);
                }
                else
                {
                    keyCollision = true;
                }
            }
        }

        // IDictionary
        public TValue this[TKey key]
        {
            get => _dict[key];
            set
            {
                _dict[key] = value;
                if (_indexByKey.TryGetValue(key, out int index))
                {
                    list[index] = new KeyValuePair(key, value);
                }
                else
                {
                    list.Add(new KeyValuePair(key, value));
                    _indexByKey.Add(key, list.Count - 1);
                }
            }
        }

        public ICollection<TKey> Keys => _dict.Keys;
        public ICollection<TValue> Values => _dict.Values;

        public void Add(TKey key, TValue value)
        {
            _dict.Add(key, value);
            list.Add(new KeyValuePair(key, value));
            _indexByKey.Add(key, list.Count - 1);
        }

        public bool ContainsKey(TKey key) => _dict.Count > 0 ? _dict.ContainsKey(key) : Keys.Contains(key);

        public bool Remove(TKey key)
        {
            if (_dict.Count > 0)
            {
                if (!_dict.Remove(key))
                    return false;

                var index = _indexByKey[key];
                list.RemoveAt(index);
                UpdateIndexLookup(index);
                _indexByKey.Remove(key);
                return true;
            }

            foreach (var kvp in list)
            {
                if (!key.Equals(kvp.Key))
                    continue;

                var index = _indexByKey[kvp.Key];
                list.RemoveAt(index);
                UpdateIndexLookup(index);
                _indexByKey.Remove(kvp.Key);
                return true;
            }
            return false;
        }

        // public bool TryRemoveByValue(TValue value)
        // {
        //     foreach (var kvp in list)
        //     {
        //         if (!value.Equals(kvp.Value))
        //             continue;
        //
        //         var index = _indexByKey[kvp.Key];
        //         list.RemoveAt(index);
        //         UpdateIndexLookup(index);
        //         _indexByKey.Remove(kvp.Key);
        //         return true;
        //     }
        //
        //     return false;
        // }

        private void UpdateIndexLookup(int removedIndex)
        {
            for (int i = removedIndex; i < list.Count; i++)
            {
                var key = list[i].Key;
                _indexByKey[key]--;
            }
        }

        public bool TryGetValue(TKey key, out TValue value) => _dict.TryGetValue(key, out value);

        // ICollection
        public int Count => _dict.Count;
        public bool IsReadOnly { get; set; }

        public void Add(KeyValuePair<TKey, TValue> pair)
        {
            Add(pair.Key, pair.Value);
        }

        public void Clear()
        {
            _dict.Clear();
            list.Clear();
            _indexByKey.Clear();
        }

        public bool ContainsValue(TValue value, out TKey key)
        {
            foreach (var kvp in list)
            {
                if (!value.Equals(kvp.Value))
                    continue;

                key = kvp.Key;
                return true;
            }
            key = default;
            return false;
        }

        public bool Contains(KeyValuePair<TKey, TValue> pair)
        {
            return _dict.TryGetValue(pair.Key, out TValue value) && EqualityComparer<TValue>.Default.Equals(value, pair.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentException("The array cannot be null.");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("The starting array index cannot be negative.");
            if (array.Length - arrayIndex < _dict.Count)
                throw new ArgumentException("The destination array has fewer elements than the collection.");

            foreach (var pair in _dict)
            {
                array[arrayIndex] = pair;
                arrayIndex++;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> pair)
        {
            if (!_dict.TryGetValue(pair.Key, out TValue value))
                return false;

            bool valueMatch = EqualityComparer<TValue>.Default.Equals(value, pair.Value);
            return valueMatch && Remove(pair.Key);

        }

        // IEnumerable
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dict.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _dict.GetEnumerator();
    }
}