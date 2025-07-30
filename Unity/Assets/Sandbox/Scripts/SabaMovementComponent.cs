using UnityEngine;

using NaughtyAttributes;

namespace Saba {
	[RequireComponent(typeof(Rigidbody2D))]
	public class SabaMovementComponent : MonoBehaviour {
		new Rigidbody2D rigidbody;
		public float Mass => rigidbody.mass;

		[ReadOnly]
		public Vector2 Velocity;
		[Min(0.01f)]
		public float AccelerationToMaxSpeedSeconds = 1f;

		Vector2 Force;	// force acting on this movement component this frame

		void Awake() { rigidbody = GetComponent<Rigidbody2D>(); }

		public void ApplyForce(Vector2 Force) => this.Force += Force;
		public void Stop() { this.Force = this.Velocity = Vector2.zero; }

		void FixedUpdate() {
			Velocity += Force / Mass;

			Vector2 nextPosition =
				(Vector2)transform.position + Velocity * Time.fixedDeltaTime;
			rigidbody.MovePosition(nextPosition);

			Force = Vector2.zero;
		}
	}
}
