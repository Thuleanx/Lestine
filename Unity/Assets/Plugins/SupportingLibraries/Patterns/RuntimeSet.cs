using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrettyPatterns {
    public class RuntimeSet<T> : ScriptableObject, ISerializationCallbackReceiver, IEnumerable<T> {
        [System.NonSerialized]
        HashSet<T> collection = new HashSet<T>();

        public int Count => collection.Count;

        public IEnumerator<T> GetEnumerator() => collection.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public delegate void ItemHandler(T item);
        public event ItemHandler OnAdd;
        public event ItemHandler OnRemove;

        public void Add(T item) {
            if (collection.Add(item)) {
                OnAdd?.Invoke(item);
            }
        }

        public void Remove(T item) {
            if (collection.Remove(item))
                OnRemove?.Invoke(item);
        }

        public void OnAfterDeserialize() => Reset();
        public void OnBeforeSerialize() => Reset();

        void Reset() {
            OnAdd = null;
            OnRemove = null;
            collection.Clear();
        }
    }
}

