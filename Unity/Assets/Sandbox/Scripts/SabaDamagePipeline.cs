using UnityEngine;
using UnityEngine.Assertions;
using System;

namespace Saba {
	[System.Serializable]
	public struct Hit {
		public SabaEntity target;
		public SabaEntity attacker;
		public SabaAttack attack;
		public Vector2 impactLocation;
		public Vector2 impactDirection;
	}

	[System.Serializable]
	public enum HitResultType : byte { Evaded, Hit, CriticalHit }

	[System.Serializable]
	public struct HitResult {
		public HitResultType type;
		public float damagePostMitigation;
		public float impulse;
	}

	public static class SabaDamagePipeline {
		const float MAX_DAMAGE_REDUCTION = 0.9f;
		const float DEFENSE_EFFECTIVENESS_COEF = 5.0f;

		public static HitResult[] CalculateHit(Span<Hit> hits) {
			int numHits = hits.Length;
			HitResult[] result = new HitResult[numHits];

			for (int i = 0; i < numHits; i++) {
				Hit hit = hits[i];
				int targetAttributes = hit.target.Attributes;
				int attackerAttributes = hit.attacker.Attributes;

				float rawDamage = SabaAliases.damage[attackerAttributes].ApplyToBase(hit.attack.BaseDamage);

				float targetDefense = SabaAliases.defense[targetAttributes];
				float rawDamageReduction = SabaAliases.defense[targetAttributes];
				float rawDefense = SabaAliases.defense[targetAttributes];

				float damageReductionFromDefense = rawDefense / (rawDefense + DEFENSE_EFFECTIVENESS_COEF * rawDamage);
				float damageReduction = Mathf.Min(rawDamageReduction, MAX_DAMAGE_REDUCTION);
				Assert.IsTrue(
					damageReduction >= 0, "Damage reduction " + damageReduction + " stat appears to be negative"
				);

				float damagePostMitigation =
					rawDamage > 0 ? rawDamage * (1 - damageReductionFromDefense) * (1 - damageReduction) : rawDamage;

				result[i] = new HitResult() {
					type = HitResultType.Hit,
					damagePostMitigation = damagePostMitigation,
					impulse = 1,
				};
			}

			return result;
		}
	}
}
