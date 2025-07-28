using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

using ADammy;
using PrettyPatterns;
using MathUtils;

using Scriptables;

namespace Saba {
	[RequireComponent(typeof(SabaEntity))]
	public class SabaPlayer : Singleton<SabaPlayer> {
		[SerializeField]
		ScriptableVector2 movementInput;
		[SerializeField]
		ScriptableVector2 mouseInput;
		[SerializeField]
		Camera mainCamera;
		[SerializeField]
		float attacksPerMinute;
		[SerializeField]
		SabaBulletBatch bulletBatch;

		SabaEntity entity;
		bool wantsToFire = false;
		EventBinding<AttackAction> attackActionBinding;
		float attackCooldown;

		public override void Awake() {
            base.Awake();
			if (!mainCamera) mainCamera = Camera.main;

			entity = GetComponent<SabaEntity>();

			attackActionBinding = new EventBinding<AttackAction>((attack) => {
				wantsToFire = attack.active;
				attackCooldown = 0;
			});
		}

		void OnEnable() {
			wantsToFire = false;
			EventBus<AttackAction>.Register(attackActionBinding);
            SabaHealthUIManager.instance.Track(entity);
		}

		void OnDisable() {
			wantsToFire = false;
			EventBus<AttackAction>.Deregister(attackActionBinding);

            // unloading the scene, so we shouldn't be calling functions even when it's referencing
            if (!gameObject.scene.isLoaded) return;
            SabaHealthUIManager.instance.Untrack(entity);
		}

		void Update() {
			Vector3 right = mainCamera.transform.right;
			Vector3 forward = mainCamera.transform.forward;
			right.z = 0;
			forward.z = 0;
			right = right.normalized;
			forward = forward.normalized;

			Vector3 desiredMoveDirection =
				right * movementInput.Value.x + forward * movementInput.Value.y;

			transform.position += entity.Attributes.MovementSpeed * Time.deltaTime *
								  desiredMoveDirection;

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

                Vector2 direction = aimPosition - transform.position;

                for (int i = -3; i <= 3; i++) {
                    Vector2 instanceDirection = Mathx.Rotate(direction, i*(Mathf.PI/16));
                    bulletBatch.InstantiateBullet(
                        transform.position,
                        instanceDirection,
                        entity.Attributes.Attack,
                        0.1f,
                        -attackCooldown);
                }
				attackCooldown += totalCooldownTime;
			}

			// if we somehow lag spike too long,
			if (attackCooldown < 0) attackCooldown = 0;
		}
	}
}
