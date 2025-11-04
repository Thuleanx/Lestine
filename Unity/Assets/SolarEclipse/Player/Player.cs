using UnityEngine;
using UnityEngine.Assertions;

using eclipse.movement;
using eclipse.interactable;
using eclipse.ui;
using PrettyPatterns;
using ADammy;

namespace eclipse.player {
    public static class PlayerTransform {
        public static Transform Value;
    }

	[RequireComponent(typeof(Entity))]
	[RequireComponent(typeof(MovementComponent))]
    [RequireComponent(typeof(InteractionSource))]
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

        [SerializeField]
        float attacksPerMinute;

		Camera mainCamera;
		Entity entity;
		MovementComponent movementComponent;
        InteractionSource interactComponent;

        [SerializeField]
        projectile.ProjectilePool bulletPool;
		bool wantsToFire = false;
		EventBinding<eclipse.input.AttackAction> attackActionBinding;
        EventBinding<eclipse.input.InteractionAction> interactActionBinding;
		float attackCooldown;

		void Awake() {
			entity = GetComponent<Entity>();
            movementComponent = GetComponent<MovementComponent>();
            interactComponent = GetComponent<InteractionSource>();

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

			attackActionBinding = new EventBinding<eclipse.input.AttackAction>((attack) => {
				wantsToFire = attack.active;
				attackCooldown = 0;
			});
            interactActionBinding = new EventBinding<eclipse.input.InteractionAction>((_event) => {
                interactComponent.TryInteract();
            });
		}

		void OnEnable() {
			wantsToFire = false;
            attackActionBinding.Bind();

            interactComponent.OnInteractableChange.AddListener(OnInteractableChanged);
		}

		void OnDisable() {
			wantsToFire = false;
            attackActionBinding.Unbind();

            interactComponent.OnInteractableChange.RemoveListener(OnInteractableChanged);

			// unloading the scene, so we shouldn't be calling functions even
			// when it's referencing
			if (!gameObject.scene.isLoaded) return;
		}

		void Update() { 
            UpdateMovement(); 
            if (wantsToFire) {
                UpdateFiring();
				attackCooldown -= Time.deltaTime;
            }
        }

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

        void UpdateFiring() {
			const int MAX_ATTACKS_PER_FRAME = 30;

			Ray mouseRay = mainCamera.ScreenPointToRay(eclipse.input.PointerPosition.Value);
			Plane plane = new Plane(Vector3.forward, transform.position);
			bool planeRayHit = plane.Raycast(mouseRay, out float mouseRayDistance);

			Assert.IsTrue(
				planeRayHit,
				"Unless our perspective / camera is incorrectly set up, we'll always point to a valid location on the plane"
			);

			Vector3 aimPosition = mouseRayDistance * mouseRay.direction + mouseRay.origin;

			float SECONDS_IN_MINUTES = 60.0f;
			float totalCooldownTime = SECONDS_IN_MINUTES / attacksPerMinute;

			for (int _ = 0; _ < MAX_ATTACKS_PER_FRAME && attackCooldown <= 0; _++) {
                Vector2 direction = aimPosition - entity.transform.position;
                direction.Normalize();
                bulletPool.InstantiateBullet(entity, entity.transform.position, direction, attackCooldown);

				attackCooldown += totalCooldownTime;
			}

			// if we somehow lag spike too long,
			if (attackCooldown < 0) attackCooldown = 0;
        }

        void OnInteractableChanged(Interactable from, Interactable to) {
            if (to == null)
                EventBus<FocusInteractableDrop>.Raise();
            else {
                EventBus<FocusInteractableChange>.Raise(new FocusInteractableChange() {
                    sprite = to.GetInteractionSprite(),
                    prompt = to.GetInteractionPrompt(),
                    location = to.transform.position
                });
            }
        }
	}
}
