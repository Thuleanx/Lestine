using UnityEngine;

using NaughtyAttributes;

namespace eclipse.movement {
	[RequireComponent(typeof(Rigidbody2D))]
	public class MovementComponent : MonoBehaviour {
		new Rigidbody2D rigidbody;

		[field:SerializeField, ReadOnly]
		public Vector2 velocity {get; private set;}

		void Awake() { rigidbody = GetComponent<Rigidbody2D>(); }

        public void ApplyForce(Vector2 force) {
            velocity += force;
        }

		void FixedUpdate() {
			Vector2 nextPosition = (Vector2)transform.position + velocity * Time.fixedDeltaTime;
			rigidbody.MovePosition(nextPosition);
		}
	}
}
