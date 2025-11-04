using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stats {
	[System.Serializable]
	public struct Modifiers {
		public float added;
		public float increase;
		public float more;

		public static Modifiers Create() { return new Modifiers { more = 1.0f }; }
		public float ApplyToBase(float @base) => (@base + added) * (1.0f + increase) * more;

		public static Modifiers operator +(Modifiers a, Modifiers b) => new Modifiers() {
			added = a.added + b.added, increase = a.increase + b.increase, more = a.more * b.more
		};
	}

}
