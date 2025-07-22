using UnityEngine;

namespace Saba {
	public class SabaBullet : MonoBehaviour {
		public Vector3 StartPosition { get; private set; }
		public Vector3 StartVelocity { get; private set; }
		public float StartTime { get; private set; }
        public float Damage {get; private set; }

		public void Initialize(Vector3 position, Vector3 startVelocity, float damage, float timeTravelled) {
			this.StartPosition = position;
			this.StartVelocity = startVelocity;
			this.StartTime = Time.time - timeTravelled;
            this.Damage = damage;
		}

		public Vector3 PositionAt(float time
		) => (time - StartTime) * StartVelocity + StartPosition;
	}
}
