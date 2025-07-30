using UnityEngine;

using MathUtils;
using NaughtyAttributes;

namespace Saba {
	[RequireComponent(typeof(Rigidbody2D))]
	public class SabaMovementComponent : MonoBehaviour {
		const float VERY_SMALL_KNOCKBACK = 0.001f;
		const float KNOCKBACK_ALPHA = 8.0f;

		new Rigidbody2D rigidbody;
		public float Mass => rigidbody.mass;

		[ReadOnly]
		public Vector2 Velocity;
		[Min(0.01f)]
		public float AccelerationToMaxSpeedSeconds = 1f;

		Vector2 Force;	// force acting on this movement component this frame
		Vector2 Knockback;

		void Awake() { rigidbody = GetComponent<Rigidbody2D>(); }

		public void ApplyForce(Vector2 Force) => this.Force += Force;
		public void ApplyKnockback(Vector2 Knockback) => this.Knockback
			+= Knockback / Mass;

		public void Stop() { this.Force = this.Velocity = Vector2.zero; }

		void FixedUpdate() {
			Velocity += Force / Mass;

			bool isKnockbackTooSmall =
				Knockback.sqrMagnitude <
				VERY_SMALL_KNOCKBACK * VERY_SMALL_KNOCKBACK;

			Vector2 frameKnockback = Vector2.zero;

			if (isKnockbackTooSmall) {
				Knockback = Vector2.zero;
			} else {
				float frameKnockbackPercentage = Mathx.Damp(
					Mathf.Lerp, 0.0f, 1.0f, KNOCKBACK_ALPHA, Time.fixedDeltaTime
				);
				frameKnockback = frameKnockbackPercentage * Knockback;

				Knockback -= frameKnockback;
			}

			Vector2 nextPosition = (Vector2)transform.position +
								   Velocity * Time.fixedDeltaTime +
								   frameKnockback;
			rigidbody.MovePosition(nextPosition);

			Force = Vector2.zero;
		}
	}
}
