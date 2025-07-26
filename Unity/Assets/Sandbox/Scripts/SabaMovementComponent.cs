using UnityEngine;

using NaughtyAttributes;

namespace Saba {
    [RequireComponent(typeof(Rigidbody))]
	public class SabaMovementComponent : MonoBehaviour {
        new Rigidbody rigidbody;
        public float Mass => rigidbody.mass;

        [ReadOnly]
        public Vector3 Velocity;
        [Min(0.01f)]
        public float AccelerationToMaxSpeedSeconds = 1f;

        Vector3 Force; // force acting on this movement component this frame

        void Awake() {
            rigidbody = GetComponent<Rigidbody>();
        }

        public void ApplyForce(Vector3 Force) => this.Force += Force;

        void FixedUpdate() {
            Velocity += Force / Mass;

            Vector3 nextPosition = transform.position + Velocity * Time.fixedDeltaTime;
            rigidbody.MovePosition(nextPosition);

            Force = Vector3.zero;
        }
	}
}
