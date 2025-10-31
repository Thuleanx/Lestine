using PrettyPatterns;

namespace eclipse.hit {
	public struct PostMitigatedHit {
        public enum Type : byte { Evaded, Hit, CriticalHit }

        public Type type;
        public float damage;
        public float impulse;
	}
}
