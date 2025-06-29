using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Scriptables {
    [CreateAssetMenu(fileName = "Data",
                    menuName = "~/Scriptable/GameObjectSet", order = 1)]
    public class ScriptableSet : GenericScriptableSet<GameObject> {}

    public class GenericScriptableSet<T> : Scriptable<List<T>>, IEnumerable<T> {
        public void Add(T obj) {
            if (Value == null) Value = new List<T>();
            Value.Add(obj);
        }

        public void Remove(T obj) {
            if (Value != null)
                Value.Remove(obj);
        }

        public IEnumerator<T> GetEnumerator() { 
            if (Value != null) return Value.GetEnumerator();
            return null;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
