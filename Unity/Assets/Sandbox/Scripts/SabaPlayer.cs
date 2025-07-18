using UnityEngine;
using UnityEngine.Assertions;

using ADammy;

using Scriptables;

namespace Saba {
	public class SabaPlayer : MonoBehaviour {
		[SerializeField]
		ScriptableVector2 movementInput;
        [SerializeField]
        ScriptableVector2 mouseInput;
		[SerializeField]
		Camera mainCamera;
		[SerializeField]
		float speed;
        [SerializeField]
        float attacksPerMinute;
        [SerializeField]
        SabaBulletBatch bulletBatch;

        bool isFiring = false;
        EventBinding<AttackAction> attackActionBinding;
        float attackCooldown;

		void Awake() {
			if (!mainCamera) mainCamera = Camera.main;

            attackActionBinding = new EventBinding<AttackAction>((attack) => {
                isFiring = attack.active;
            });
		}

        void OnEnable() {
            isFiring = false;
            EventBus<AttackAction>.Register(attackActionBinding);
        }

        void OnDisable() {
            isFiring = false;
            EventBus<AttackAction>.Deregister(attackActionBinding);
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

			transform.position += speed * Time.deltaTime * desiredMoveDirection;

            bool canFire = isFiring && attackCooldown <= 0;
            if (canFire) Fire();

            attackCooldown -= Mathf.Min(Time.deltaTime, attackCooldown);
		}

        void Fire() {
            Ray mouseRay = mainCamera.ScreenPointToRay(mouseInput.Value);
            Plane plane = new Plane(Vector3.up, transform.position);
            bool planeRayHit = plane.Raycast(mouseRay, out float mouseRayDistance);

            Assert.IsTrue(planeRayHit, "Unless our perspective / camera is incorrectly set up, we'll always point to a valid location on the plane");

            Vector3 aimPosition = mouseRayDistance * mouseRay.direction + mouseRay.origin;

            bulletBatch.InstantiateBullet(transform.position, aimPosition);

            float SECONDS_IN_MINUTES = 60.0f;
            attackCooldown = SECONDS_IN_MINUTES / attacksPerMinute;
        }
	}
}
