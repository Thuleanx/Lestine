using UnityEngine;

using PrettyPatterns;
using eclipse.movement;

namespace eclipse.player {
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
		}

		void Update() { UpdateMovement(); }

		[Header("Movement")]
		[SerializeField]
		float accelerationToMaxSpeed = 0.5f;

		void UpdateMovement() {
			Vector2 right = mainCamera.transform.right;
			Vector2 forward = mainCamera.transform.forward;

			Vector2 movementInput = eclipse.input.Movement.Value.normalized;

			Vector2 desiredDirection = right * movementInput.x + forward * movementInput.y;
			float movementSpeed = Alias.movementSpeed[entity.stats];
			Vector2 desiredVelocity = desiredDirection * movementSpeed;

			Vector2 desiredForce = desiredVelocity - movementComponent.velocity;
			float accelerationClamp = movementSpeed / accelerationToMaxSpeed;

			Vector2 appliedForce = Vector2.ClampMagnitude(desiredForce, accelerationClamp);
            movementComponent.ApplyForce(appliedForce);
		}
	}
}
