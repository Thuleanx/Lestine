using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

using ADammy;

using Scriptables;

namespace Saba {
	[RequireComponent(typeof(SabaEntity))]
	public class SabaPlayer : MonoBehaviour {
		[SerializeField]
		ScriptableVector2 movementInput;
		[SerializeField]
		ScriptableVector2 mouseInput;
		[SerializeField]
		Camera mainCamera;
		[SerializeField]
		float attacksPerMinute;
        [SerializeField]
        float executionRange;
		[SerializeField]
		SabaBulletBatch bulletBatch;
		[SerializeField]
		SabaEntity entity;

		bool wantsToFire = false;
		EventBinding<AttackAction> attackActionBinding;
		float attackCooldown;

		EventBinding<ExecutionAction> executionActionBinding;

		void Awake() {
			if (!mainCamera) mainCamera = Camera.main;

			entity = GetComponent<SabaEntity>();

			attackActionBinding = new EventBinding<AttackAction>((attack) => {
				wantsToFire = attack.active;
				attackCooldown = 0;
			});
            executionActionBinding = new EventBinding<ExecutionAction>((execution) => {
                HandlesExecution();
            });
		}

		void OnEnable() {
			wantsToFire = false;
			EventBus<AttackAction>.Register(attackActionBinding);
            EventBus<ExecutionAction>.Register(executionActionBinding);
		}

		void OnDisable() {
			wantsToFire = false;
			EventBus<AttackAction>.Deregister(attackActionBinding);
            EventBus<ExecutionAction>.Deregister(executionActionBinding);
		}

		void Update() {
			Vector3 right = mainCamera.transform.right;
			Vector3 forward = mainCamera.transform.forward;
			right.y = 0;
			forward.y = 0;
			right = right.normalized;
			forward = forward.normalized;

			Vector3 desiredMoveDirection =
				right * movementInput.Value.x + forward * movementInput.Value.y;

			transform.position += entity.Stats.MovementSpeed * Time.deltaTime *
								  desiredMoveDirection;

			if (wantsToFire) {
				HandlesFiring();
				attackCooldown -= Time.deltaTime;
			}
		}

		void HandlesFiring() {
			const int MAX_ATTACKS_PER_FRAME = 30;

			Ray mouseRay = mainCamera.ScreenPointToRay(mouseInput.Value);
			Plane plane = new Plane(Vector3.up, transform.position);
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
				bulletBatch.InstantiateBullet(
					transform.position,
					aimPosition - transform.position,
					entity.Stats.Attack,
					-attackCooldown
				);
				attackCooldown += totalCooldownTime;
			}

			// if we somehow lag spike too long,
			if (attackCooldown < 0) attackCooldown = 0;
		}

		void HandlesExecution() {
            SabaExecutableEntity closestEntity = null;
            float closestSqDistance = float.MaxValue;

			foreach (SabaExecutableEntity executable in
						 SabaExecutableRuntimeGroup.instance
							 .activeEdibleEnemies) {
                Vector3 displacement = executable.transform.position - transform.position;
                float sqDistance = Vector3.Dot(displacement, displacement);

                if (closestSqDistance > sqDistance) {
                    closestSqDistance = sqDistance;
                    closestEntity = executable;
                }
            }

            if (!closestEntity) return;

            entity.Stats.MovementSpeed += 5;
            Destroy(closestEntity.gameObject);
		}
	}
}
