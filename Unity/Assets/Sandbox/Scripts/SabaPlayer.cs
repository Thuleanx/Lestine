using UnityEngine;
using UnityEngine.Assertions;

using ADammy;
using PrettyPatterns;

using Scriptables;

namespace Saba {
	[RequireComponent(typeof(SabaEntity))]
	[RequireComponent(typeof(SabaMovementComponent))]
	public class SabaPlayer : SingletonNullable<SabaPlayer> {
		[SerializeField]
		ScriptableVector2 movementInput;
		[SerializeField]
		ScriptableVector2 mouseInput;
		[SerializeField]
		Camera mainCamera;
		[SerializeField]
		float attacksPerMinute;
		[SerializeField, Min(0.01f)]
		float secondsToTopSpeed = 0.5f;
        [SerializeField]
        SabaShootAbility shootAbility;

		[System.NonSerialized]
		public SabaEntity entity;
		[System.NonSerialized]
		public SabaMovementComponent movementComponent;

		bool wantsToFire = false;
		EventBinding<AttackAction> attackActionBinding;
		float attackCooldown;

		public override void Awake() {
			base.Awake();
			if (!mainCamera) mainCamera = Camera.main;

			entity = GetComponent<SabaEntity>();
			movementComponent = GetComponent<SabaMovementComponent>();

			attackActionBinding = new EventBinding<AttackAction>((attack) => {
				wantsToFire = attack.active;
				attackCooldown = 0;
			});
		}

		void OnEnable() {
			wantsToFire = false;
			EventBus<AttackAction>.Register(attackActionBinding);
		}

		void OnDisable() {
			wantsToFire = false;
			EventBus<AttackAction>.Deregister(attackActionBinding);

			// unloading the scene, so we shouldn't be calling functions even
			// when it's referencing
			if (!gameObject.scene.isLoaded) return;
		}

		void Update() {
			Vector2 right = mainCamera.transform.right;
			Vector2 forward = mainCamera.transform.forward;
			// normalize right here after the z component has dropped

			Vector2 desiredMoveDirection =
				right * movementInput.Value.x + forward * movementInput.Value.y;
            desiredMoveDirection = desiredMoveDirection.normalized;

			Vector2 desiredMoveVelocity =
				desiredMoveDirection * entity.Attributes.MovementSpeed;

			Vector2 desiredForce =
				desiredMoveVelocity - movementComponent.Velocity;

			float accelerationMax =
				entity.Attributes.MovementSpeed / secondsToTopSpeed;

			Vector2 appliedForce = Vector2.ClampMagnitude(
				desiredForce,
				Time.deltaTime * accelerationMax * movementComponent.Mass
			);

			movementComponent.ApplyForce(appliedForce);

			if (wantsToFire) {
				HandlesFiring();
				attackCooldown -= Time.deltaTime;
			}
		}

		void HandlesFiring() {
			const int MAX_ATTACKS_PER_FRAME = 30;

			Ray mouseRay = mainCamera.ScreenPointToRay(mouseInput.Value);
			Plane plane = new Plane(Vector3.forward, transform.position);
			bool planeRayHit =
				plane.Raycast(mouseRay, out float mouseRayDistance);

			Assert.IsTrue(
				planeRayHit,
				"Unless our perspective / camera is incorrectly set up, we'll always point to a valid location on the plane"
			);

			Vector3 aimPosition =
				mouseRayDistance * mouseRay.direction + mouseRay.origin;

			float SECONDS_IN_MINUTES = 60.0f;
			float totalCooldownTime = SECONDS_IN_MINUTES / attacksPerMinute;

			for (int _ = 0; _ < MAX_ATTACKS_PER_FRAME && attackCooldown <= 0;
				 _++) {

                shootAbility.Activate(entity, (Vector2) aimPosition, attackCooldown);

                attackCooldown += totalCooldownTime;
			}

			// if we somehow lag spike too long,
			if (attackCooldown < 0) attackCooldown = 0;
		}
	}
}
