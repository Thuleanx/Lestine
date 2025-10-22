using UnityEngine;
using UnityEngine.Assertions;

using PrettyPatterns;

namespace Saba {
	[RequireComponent(typeof(SabaEntity))]
	public class SabaEntityStatsInitializer : MonoBehaviour {
		[SerializeField]
		float maxHealth;
		[SerializeField]
		float defense;
		[SerializeField]
		float damageReduction;
		[SerializeField]
		float movementSpeed;

		void OnEnable() {
			SabaEntity entity = GetComponent<SabaEntity>();

			entity.Attributes = (SabaAliases.allStats.coreStats as RemovableSpanList).Allocate(1);
			entity.Resource = (SabaAliases.allStats.coreResource as RemovableSpanList).Allocate(1);
			Assert.IsTrue(entity.Attributes == entity.Resource);

			SabaAliases.maxHealth[entity.Attributes] = maxHealth;
			SabaAliases.defense[entity.Attributes] = defense;
			SabaAliases.damageReduction[entity.Attributes] = damageReduction;
			SabaAliases.movementSpeed[entity.Attributes] = movementSpeed;
			SabaAliases.coreStats.entities[entity.Attributes] = entity;

			SabaAliases.allStats.InitializeResource(entity.Attributes, entity.Resource);
		}
	}
}
