using UnityEngine;
using PrettyPatterns;

namespace eclipse.hit {
    [System.Serializable]
	public struct Hit {
        public Entity target;
        public Entity attacker;
        public FrugalList<float> baseDamage;
        public float knockback;
        public Vector2 location;
        public Vector2 direction;
	}
}
