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
				SabaAttributes targetAttribute = hit.target.Attributes;
				SabaAttributeCoefficients damageScaling = hit.attacker.Attributes.DamageScaling;

				float rawDamage = damageScaling.Apply(hit.attack.BaseDamage);

				float damageReductionFromDefense =
					targetAttribute.Defense / (targetAttribute.Defense + DEFENSE_EFFECTIVENESS_COEF * rawDamage);
				float rawDamageReduction = Mathf.Min(targetAttribute.DamageReduction, MAX_DAMAGE_REDUCTION);
				Assert.IsTrue(
					rawDamageReduction >= 0, "Damage reduction " + rawDamageReduction + " stat appears to be negative"
				);

				float damagePostMitigation =
					rawDamage > 0 ? rawDamage * (1 - damageReductionFromDefense) * (1 - rawDamageReduction) : rawDamage;

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
