using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace eclipse.hit {
	public static class DamageResolver {
		const float MAX_DAMAGE_REDUCTION = 0.9f;
		const float DEFENSE_EFFECTIVENESS_COEF = 5.0f;

		public static PostMitigatedHit[] Process(Span<Hit> hits) {
			int num = hits.Length;
			PostMitigatedHit[] result = new PostMitigatedHit[num];

			for (int i = 0; i < num; i++) {
				Hit hit = hits[i];
				int targetAttributes = hit.target.stats;
				int attackerAttributes = hit.attacker.stats;

				foreach (float rawDamage in hit.baseDamage) {
					float rawDamageReduction = Alias.damageReduction[targetAttributes];
					float rawDefense = Alias.defense[targetAttributes];

					float damageReductionFromDefense =
						rawDefense / (rawDefense + DEFENSE_EFFECTIVENESS_COEF * rawDamage);
					float damageReduction = Mathf.Min(rawDamageReduction, MAX_DAMAGE_REDUCTION);
					Assert.IsTrue(
						damageReduction >= 0, "Damage reduction " + damageReduction + " stat appears to be negative"
					);

					float damagePostMitigation =
						rawDamage > 0 ? rawDamage * (1 - damageReductionFromDefense) * (1 - damageReduction)
									  : rawDamage;

					result[i] = new PostMitigatedHit() {
						type = PostMitigatedHit.Type.Hit,
						damage = damagePostMitigation,
						impulse = 1,
					};
				}
			}

			return result;
		}
	}
}
