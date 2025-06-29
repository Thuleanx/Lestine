using System;
using UnityEngine;
using System.Collections.Generic;

namespace PrettyPatterns {
    [Serializable]
    /// Requires Unity 2020.1+
    public struct Optional<T>
    {
        [SerializeField] private bool enabled;
        [SerializeField] private T value;

        public bool IsValid => enabled;
        public T Value => value;

        public Optional(T initialValue)
        {
            enabled = true;
            value = initialValue;
        }

        // Returns an optional wrapper around said value, 
        // or {} if said value is null
        public static Optional<T> Wrap(T initialValue) 
            => initialValue == null ? new Optional<T>() : new Optional<T>(initialValue);

        public static implicit operator Optional<T>(T t) => new Optional<T>(t);

        public IEnumerable<T> Iter() {
            if (enabled) yield return value;
        }
    }
}
