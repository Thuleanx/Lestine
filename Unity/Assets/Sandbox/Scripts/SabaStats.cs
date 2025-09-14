[System.Serializable]
public struct SabaAttributes {
	public float MaxHealth;
	public float Defense;
	public float DamageReduction;
	public float MovementSpeed;
}

[System.Serializable]
public struct SabaAttributeScaling {
    public SabaAttributeCoefficients MaxHealth;
    public SabaAttributeCoefficients Defense;
    public SabaAttributeCoefficients DamageReduction;
    public SabaAttributeCoefficients MovementSpeed;
}

[System.Serializable]
public struct SabaAttributeCoefficients {
	public float Added;
	public float Increase;
	public float More;

	public static SabaAttributeCoefficients operator +(SabaAttributeCoefficients lt, SabaAttributeCoefficients rt) =>
		new SabaAttributeCoefficients() {
			Added = lt.Added + rt.Added, Increase = lt.Increase + rt.Increase, More = lt.More * rt.More
		};

	public static SabaAttributeCoefficients operator -(SabaAttributeCoefficients lt, SabaAttributeCoefficients rt) =>
		new SabaAttributeCoefficients() {
			Added = lt.Added + rt.Added, Increase = lt.Increase - rt.Increase, More = lt.More / rt.More
		};

    public float Apply(float Base) => (Base + Added) * (1.0f + Increase) * More;
}

[System.Serializable]
public struct SabaSingleAttribute {
	public float Base;
}

[System.Serializable]
public struct SabaResource {
	public float Health;
}
