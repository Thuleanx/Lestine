using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PrettyPatterns {
	public class FrugalListEnumerator<T> : IEnumerator<T>
		where T : IEquatable<T> {
		FrugalList<T> list;
		int index = -1;

		public T Current => list[index];
		object IEnumerator.Current => Current;

		public FrugalListEnumerator(FrugalList<T> list) { this.list = list; }

		public void Dispose() {}

		public bool MoveNext() {
			index++;
			return index < list.Count;
		}

		public void Reset() { index = -1; }
	}

    [System.Serializable]
	public struct FrugalList<T> : IList<T>
		where T : IEquatable<T> {

        [SerializeField]
        bool hasFirst;

        [SerializeField]
		T first;

        [SerializeField]
		List<T> rest;

        public FrugalList(T item) {
            hasFirst = true;
            first = item;
            rest = null;
        }

        public static implicit operator FrugalList<T>(T t) => new FrugalList<T>(t);

		public T this[int index] {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get {
				if (index == 0 && hasFirst) {
					return first;
				} else if (index > 0 && rest != null && index - 1 < rest.Count) {
					return rest[index - 1];
				} else {
					throw new IndexOutOfRangeException("Index " + index + " out of range of [0, " + Count + ")");
				}
			}
			set {
				if (index == 0 && hasFirst) {
					first = value;
				} else if (index > 0 && rest != null && index - 1 < rest.Count) {
					rest[index - 1] = value;
				} else {
					throw new IndexOutOfRangeException("Index " + index + " out of range of [0, " + Count + ")");
				}
			}
		}


		public int Count => (hasFirst ? 1 : 0) + (rest != null ? rest.Count : 0);
		public bool IsReadOnly => false;

		public void Add(T item) {
			if (!hasFirst) first = item;
			else {
				if (rest == null) rest = new List<T>();
				rest.Add(item);
			}
		}

		public void Clear() {
            hasFirst = false;
			first = default(T);
			rest = null;
		}

		public bool Contains(T item) {
			if (hasFirst && first.Equals(item)) return true;
			return rest != null && rest.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex) {
			if (hasFirst) array[arrayIndex] = first;
			if (rest != null) rest.CopyTo(array, arrayIndex + 1);
		}

		public IEnumerator<T> GetEnumerator() => new FrugalListEnumerator<T>(this);

		public int IndexOf(T item) {
			if (hasFirst && first.Equals(item)) return 0;
			if (rest != null) return rest.IndexOf(item) + 1;
			return -1;
		}

		public void Insert(int index, T item) {
			if (index == 0) {
				if (hasFirst) {
					if (rest == null) rest = new List<T>();
					rest.Insert(0, first);
				}
				first = item;
			} else {
				if (rest == null)
					throw new ArgumentOutOfRangeException("Index " + index + " out of range of [0, " + Count + ")");
				rest.Insert(index - 1, item);
			}
		}

		public bool Remove(T item) {
			if (!hasFirst) return false;

			if (item.Equals(first)) {
				RemoveAt(0);
				return true;
			} else if (rest != null && rest.Remove(item)) {
				if (rest.Count == 0) rest = null;
				return true;
			}

			return false;
		}

		public void RemoveAt(int index) {
			if (index < 0 || index >= Count)
				throw new ArgumentOutOfRangeException("Index " + index + " out of range of [0, " + Count + ")");

			if (index == 0) {
                hasFirst = false;
                first = default(T);
				if (rest != null && rest.Count > 0) {
					first = rest[0];
					rest.RemoveAt(0);
					if (rest.Count == 0) rest = null;
				}
			} else if (rest != null) {
				rest.RemoveAt(index - 1);
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
