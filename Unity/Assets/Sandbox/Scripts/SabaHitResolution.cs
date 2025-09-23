using UnityEngine;
using System.Collections.Generic;

using PrettyPatterns;

namespace Saba {
	public class SabaHitResolution : Singleton<SabaHitResolution> {
		const int MAX_HIT = 100000;

		int NumUnresolvedHits = 0;
		Hit[] UnresolvedHits = new Hit[MAX_HIT];

		public void RegisterHits(IEnumerable<Hit> hits) {
			foreach (Hit hit in hits) UnresolvedHits[NumUnresolvedHits++] = hit;
		}

		void Resolve() {
			if (NumUnresolvedHits == 0) return;

			HitResult[] results =
				SabaDamagePipeline.CalculateHit(new System.Span<Hit>(UnresolvedHits, 0, NumUnresolvedHits));

			List<SabaEntity> deadEntities = new List<SabaEntity>(NumUnresolvedHits);
			List<SabaEntity> damagedEntities = new List<SabaEntity>(NumUnresolvedHits);

			for (int i = 0; i < NumUnresolvedHits; i++) {
				Hit hit = UnresolvedHits[i];
				HitResult result = results[i];

				if (result.type == HitResultType.Hit) {
					SabaAliases.health[hit.target.Attributes] -= result.damagePostMitigation;
                    Debug.Log(hit.target + " takes " + result.damagePostMitigation + " damage");

					if (hit.target.IsDead) {
						hit.attacker.EffectDispatch?.LazyDispatch(
							new SabaGameplayEvents.Kill[] { new SabaGameplayEvents.Kill() }
						);
						deadEntities.Add(hit.target);
					} else {
						damagedEntities.Add(hit.target);
						hit.target.MovementComponent?.ApplyKnockback(hit.impactDirection * result.impulse);
					}
				}
			}

			SabaHealthUIManager.instance.OnDamageTaken(damagedEntities);
			SabaHealthUIManager.instance.OnDeath(deadEntities);
			SabaEntity.Kill(deadEntities);

			NumUnresolvedHits = 0;
		}

		public void LateUpdate() { Resolve(); }
	}
}
