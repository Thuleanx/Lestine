using UnityEngine;
using System.Collections.Generic;

using ADammy;

namespace Saba {
	[RequireComponent(typeof(SabaEntity))]
	[RequireComponent(typeof(SabaBuffContainer))]
	public class SabaExecutionAbility : MonoBehaviour {
		[SerializeField, Range(0, 10)]
		float range;
		[SerializeField, Range(0, 1)]
		float healthRefund;
        [SerializeField]
        int maxTargetsCount;

		SabaEntity entity;
        SabaBuffContainer buffContainer;
		EventBinding<ExecutionAction> executionActionBinding;

		void Awake() {
			entity = GetComponent<SabaEntity>();
            buffContainer = GetComponent<SabaBuffContainer>();

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
            // We will potentially exceed the max targets count by 1
            List<(SabaEntity, float)> bestTargets = new List<(SabaEntity, float)>(maxTargetsCount + 1);

			foreach (SabaExecutableRuntimeGroup
						 .Entry entry in SabaExecutableRuntimeGroup.instance
						 .activeEntities.Enumerate()) {
				SabaEntity executable = entry.entity;
				// It's possible for there to be null entries in this runtime group,
                // especially as we consume some enemies
				bool isTargetValid =
					executable != null && executable.isActiveAndEnabled;
				if (!isTargetValid) continue;

				Vector3 displacement =
					executable.transform.position - transform.position;
				float sqDistance = Vector3.Dot(displacement, displacement);

                bool isTargetTooFar = sqDistance > range*range;
				if (isTargetTooFar) continue;

                bestTargets.Add((entry.entity, sqDistance));

                if (bestTargets.Count > maxTargetsCount) {
                    int indexOfFurthest = 0;
                    for (int i = 1; i < bestTargets.Count; i++)
                        if (bestTargets[i].Item2 > bestTargets[indexOfFurthest].Item2)
                            indexOfFurthest = i;
                    bestTargets.RemoveAt(indexOfFurthest);
                }
			}

			bool hasValidTarget = bestTargets.Count > 0;
			if (!hasValidTarget) return;

            foreach ((SabaEntity target, float _) in bestTargets) {
                float heal = entity.Attributes.MaxHealth * healthRefund;
                heal = Mathf.Min(
                    heal, entity.Attributes.MaxHealth - entity.Resource.Health
                );
                entity.Resource.Health += heal;
                buffContainer.ApplyBuff(SabaBuffData.Make(SabaBuffType.Enlarge, 1.2f, true), 10.0f);
                buffContainer.ApplyBuff(SabaBuffData.Make(SabaBuffType.SpeedIncrease, 5.0f, false), 10.0f);

                Destroy(target.gameObject);
            }
		}
	}
}
