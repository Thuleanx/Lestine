using UnityEngine;

namespace Saba {
    public struct Hit {
        public SabaEntity Entity;
        public Vector2 ImpactLocation;
        public Vector2 Direction;
        public SabaAttack Attack;
    }

	public static class SabaDamagePipeline {
        public enum HitResult {
            Evaded,
            Hit,
            CriticalHit
        };
	}
}
