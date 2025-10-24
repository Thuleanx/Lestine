using UnityEngine;

using PrettyPatterns;
using eclipse.movement;

namespace eclipse.player {
    public static class PlayerTransform {
        public static Transform Value;
    }

	[RequireComponent(typeof(Entity))]
	[RequireComponent(typeof(MovementComponent))]
	public class Player : MonoBehaviour {
		[Header("Stats")]
		[SerializeField]
		float maxHealth;
		[SerializeField]
		float defense;
		[SerializeField]
		float damageReduction;
		[SerializeField]
		float movementSpeed;

		Camera mainCamera;
		Entity entity;
		MovementComponent movementComponent;

		void Awake() {
			entity = GetComponent<Entity>();
            movementComponent = GetComponent<MovementComponent>();

			entity.stats = (Alias.coreStats as RemovableSpanList).Allocate(1);
			entity.resource = (Alias.coreResource as RemovableSpanList).Allocate(1);

			Alias.maxHealth[entity.stats] = Alias.health[entity.resource] = maxHealth;
			Alias.defense[entity.stats] = defense;
			Alias.damageReduction[entity.stats] = damageReduction;
			Alias.movementSpeed[entity.stats] = movementSpeed;
			Alias.coreStats.entities[entity.stats] = entity;

			Alias.stats.InitializeResource(entity.stats, entity.resource);

			mainCamera = Camera.main;
            PlayerTransform.Value = transform;
		}

		void Update() { UpdateMovement(); }

		[Header("Movement")]
		[SerializeField]
		float accelerationToMaxSpeed = 0.5f;
        [SerializeField]
        float deccelerationToZero = 0.5f;

		void UpdateMovement() {
			Vector2 right = mainCamera.transform.right;
			Vector2 forward = mainCamera.transform.forward;

			Vector2 movementInput = eclipse.input.Movement.Value.normalized;

			Vector2 desiredDirection = right * movementInput.x + forward * movementInput.y;
			float movementSpeed = Alias.movementSpeed[entity.stats];
			Vector2 desiredVelocity = desiredDirection * movementSpeed;

            float accelMax = 0;
            bool isDeccelerating = desiredVelocity.sqrMagnitude <= 0.01f;
            if (isDeccelerating) {
                accelMax =  movementSpeed / deccelerationToZero;
            } else {
                accelMax = movementSpeed / accelerationToMaxSpeed;
            }

			Vector2 desiredForce = desiredVelocity - movementComponent.velocity;

			Vector2 appliedForce = Vector2.ClampMagnitude(desiredForce, accelMax * Time.deltaTime);
            movementComponent.ApplyForce(appliedForce);
		}
	}
}
