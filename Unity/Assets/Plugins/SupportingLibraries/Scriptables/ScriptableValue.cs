using UnityEngine;

namespace Scriptables {
	public class Scriptable<T> : ScriptableObject {
		[field:System.NonSerialized]
		public T Value;

		public static implicit operator T(Scriptable<T> s) => s.Value;
	}
}
