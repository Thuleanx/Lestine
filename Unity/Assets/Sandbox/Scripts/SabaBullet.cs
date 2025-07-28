using UnityEngine;

namespace Saba {
	public class SabaBullet : MonoBehaviour {
		public Vector2 StartPosition { get; private set; }
		public Vector2 StartVelocity { get; private set; }
		public float StartTime { get; private set; }
        public float Damage {get; private set; }

		public void Initialize(Vector2 position, Vector2 startVelocity, float damage, float timeTravelled) {
			this.StartPosition = position;
			this.StartVelocity = startVelocity;
			this.StartTime = Time.time - timeTravelled;
            this.Damage = damage;
		}

		public Vector2 PositionAt(float time
		) => (time - StartTime) * StartVelocity + StartPosition;
	}
}
