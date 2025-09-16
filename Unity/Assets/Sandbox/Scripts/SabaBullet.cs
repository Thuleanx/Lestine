using UnityEngine;

namespace Saba {
	public class SabaBullet : MonoBehaviour {
		public SabaEntity Owner { get; private set; }
		public Vector2 StartPosition { get; private set; }
		public Vector2 StartVelocity { get; private set; }
		public float StartTime { get; private set; }

		public void Initialize(
			SabaEntity owner, Vector2 position, Vector2 startVelocity, float timeTravelled
		) {
			this.Owner = owner;
			this.StartPosition = position;
			this.StartVelocity = startVelocity;
			this.StartTime = Time.time - timeTravelled;
		}

		public Vector2 PositionAt(float time) => (time - StartTime) * StartVelocity + StartPosition;
	}
}
