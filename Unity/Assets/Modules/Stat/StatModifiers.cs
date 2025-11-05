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

		public static Modifiers operator -(Modifiers a) => new Modifiers() {
			added = -a.added,
			increase = -a.increase,
			more = 1.0f / a.more,
		};
	}

	[System.Serializable]
	public enum ModifierEntry { eMaxHealth, eDefense, eDamageReduction, eMovementSpeed, eDamage }

}
