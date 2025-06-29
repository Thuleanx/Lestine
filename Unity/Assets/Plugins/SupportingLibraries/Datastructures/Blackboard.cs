using System.Collections;
using UnityEngine;

namespace Datastructures {
    public interface IBlackboard {
        public Hashtable Board { get; protected set; }

        public T Get<T, E>(E key) {
            if (!Board.ContainsKey(key))
                return default(T);
            return (T) Board[key];
        }

        public void Set<T, E>(E key, T item) {
            Board[key] = item;
        }
    }
}
