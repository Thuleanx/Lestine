using UnityEngine;

using PrettyPatterns;
using Stats;
using NaughtyAttributes;

namespace eclipse {
	[RequireComponent(typeof(Entity))]
	public class EntityDebugger : MonoBehaviour {
		[SerializeField, ReadOnly]
		float health;
		[SerializeField, ReadOnly]
		float movementSpeed;
		[SerializeField, ReadOnly]
		Optional<float> baseMoveSpeed;
		[SerializeField, ReadOnly]
		Optional<Modifiers> scalingMoveSpeed;

		Entity entity;

		void Awake() { entity = GetComponent<Entity>(); }

		void Update() {
			health = Alias.health[entity.resource];
			movementSpeed = Alias.movementSpeed[entity.stats];

			if (entity.extra.IsValid) {
				baseMoveSpeed = Alias.coreStatsBase.movementSpeed[entity.extra.Value.@base];
				scalingMoveSpeed = Alias.coreStatsScaling.movementSpeed[entity.extra.Value.scaling];
			} else {
				baseMoveSpeed = default;
				scalingMoveSpeed = default;
			}
		}
	}
}
