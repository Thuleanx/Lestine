using UnityEngine;

using ADammy;

namespace Saba {
	[RequireComponent(typeof(SabaEntity))]
	public class SabaExecutionAbility : MonoBehaviour {
		[SerializeField, Range(0, 10)]
		float range;
		[SerializeField, Range(0, 1)]
		float healthRefund;

		SabaEntity entity;
		EventBinding<ExecutionAction> executionActionBinding;

		void Awake() {
			entity = GetComponent<SabaEntity>();

			executionActionBinding =
				new EventBinding<ExecutionAction>((execution) => {
					Perform();
				});
		}

		void OnEnable() {
			EventBus<ExecutionAction>.Register(executionActionBinding);
		}

		void OnDisable() {
			EventBus<ExecutionAction>.Deregister(executionActionBinding);
		}

		public void Perform() {
			SabaExecutableEntity closestEntity = null;
			float closestSqDistance = float.MaxValue;

			foreach (SabaExecutableEntity executable in
						 SabaExecutableRuntimeGroup.instance
							 .activeEdibleEnemies) {
				Vector3 displacement =
					executable.transform.position - transform.position;
				float sqDistance = Vector3.Dot(displacement, displacement);

				if (closestSqDistance > sqDistance) {
					closestSqDistance = sqDistance;
					closestEntity = executable;
				}
			}

			bool hasValidTarget =
				closestEntity && closestSqDistance < range * range;
			if (!hasValidTarget) return;

			entity.Stats.MovementSpeed += 5;

			float heal = entity.Stats.MaxHealth * healthRefund;
            heal = Mathf.Min(heal, entity.Stats.MaxHealth - entity.Resource.Health);
			entity.Resource.Health += heal;

			Destroy(closestEntity.gameObject);
		}
	}
}
