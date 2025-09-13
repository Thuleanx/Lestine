using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using MathUtils;

namespace Saba {
	[CreateAssetMenu(menuName = "Saba/Ability/Shoot")]
	public class SabaShootAbility : ScriptableObject, SabaAbility {
		[SerializeField]
		SabaBulletBatch bulletBatch;

		[field:SerializeField, Min(1)]
		public int NumBullets { get; private set; } = 1;

		[field:SerializeField, Min(0)]
		public float InaccuracyAngle {
			get; private set;
		} = 0;

		[field:SerializeField, Min(0.0f)]
		public float KickbackForce {
			get; private set;
		} = 0;

		[System.NonSerialized] SabaBulletBatch runtimeBulletBatch;
		[System.NonSerialized]
		Dictionary<SabaEntity, SabaShootAbilityInstance> abilityInstances;

        [SerializeField]
        SabaAttack attackDefinition;

		public void Activate(SabaEntity entity, SabaAbility.Target target) =>
			Activate(entity, target, 0);

		// Because we can have frame lag, we oftentimes want to be able to
		// start the ability as if it was started before the current time,
		// hence the elapsed time parameter
		public void Activate(
			SabaEntity entity, SabaAbility.Target target, float elapsedTime
		) {
			if (runtimeBulletBatch == null)
				runtimeBulletBatch = Instantiate(bulletBatch);
			if (abilityInstances == null)
				abilityInstances =
					new Dictionary<SabaEntity, SabaShootAbilityInstance>();

			SabaShootAbilityInstance instance;

			if (!abilityInstances.ContainsKey(entity)) {
				instance = new SabaShootAbilityInstance(
				) { ability = this,
					entity = entity,
					movementComponent =
						entity.GetComponent<SabaMovementComponent>(),
					bulletBatch = runtimeBulletBatch };

				abilityInstances.Add(entity, instance);
			} else {
				instance = abilityInstances[entity];
			}

			instance.Activate(target, elapsedTime);
		}

		public struct SabaShootAbilityInstance : SabaAbilityInstance {
			public SabaShootAbility ability;
			public SabaEntity entity;
			public SabaMovementComponent movementComponent;
			public SabaBulletBatch bulletBatch;

			// Because we can have frame lag, we oftentimes want to be able to
			// start the ability as if it was started before the current time,
			// hence the elapsed time parameter
			public void Activate(SabaAbility.Target target, float elapsedTime) {
				Assert.AreEqual(
					entity.GetComponent<SabaMovementComponent>(),
					movementComponent,
					"entity movement component must match with supplied component"
				);
				Assert.AreEqual(
					target.type, SabaAbility.TargetingType.Location
				);

				Vector2 targetPosition = target.location;
				Vector2 entityPosition = entity.transform.position;

				Vector2 direction = targetPosition - entityPosition;

				movementComponent.ApplyKnockback(
					-direction * ability.KickbackForce
				);

				float inaccuracy = Mathf.Deg2Rad * ability.InaccuracyAngle *
								   Mathx.RandomRange(-0.5f, 0.5f);
				float offset = -ability.NumBullets / 2.0f;
				for (int i = 0; i < ability.NumBullets; i++) {
					Vector2 instanceDirection =
						Mathx.Rotate(direction, (i + offset) * (Mathf.PI / 16) + inaccuracy);
					bulletBatch.InstantiateBullet(
						entityPosition,
						instanceDirection,
						ability.attackDefinition.BaseDamage,
						ability.attackDefinition.Knockback,
						-elapsedTime
					);
				}
			}

			// Because we can attack
			public void Activate(SabaAbility.Target target
			) => Activate(target, 0);
		}
	}
}
