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

		public struct ResolutionResult {
			// Everyone in indicex [0, numDead) are dead
			// and [numDead, total) are just damaged
			public int numDead;
			public int total;
			public SabaEntity[] allHitEntities;
		}

		public ResolutionResult Resolve() {
			ResolutionResult resolutionResults = new ResolutionResult() {
				numDead = 0,
				total = 0,
				allHitEntities = new SabaEntity[NumUnresolvedHits],
			};

			if (NumUnresolvedHits == 0) return resolutionResults;

			HitResult[] results =
				SabaDamagePipeline.CalculateHit(new System.Span<Hit>(UnresolvedHits, 0, NumUnresolvedHits));

			for (int i = 0; i < NumUnresolvedHits; i++) {
				Hit hit = UnresolvedHits[i];
				HitResult result = results[i];

				if (result.type == HitResultType.Hit) {
					SabaAliases.health[hit.target.Attributes] -= result.damagePostMitigation;

					if (hit.target.IsDead) {
						hit.attacker.EffectDispatch?.LazyDispatch(
							new SabaGameplayEvents.Kill[] { new SabaGameplayEvents.Kill() }
						);
						resolutionResults.allHitEntities[resolutionResults.total++] =
							resolutionResults.allHitEntities[resolutionResults.numDead];
						resolutionResults.allHitEntities[resolutionResults.numDead++] = hit.target;
					} else {
						resolutionResults.allHitEntities[resolutionResults.total++] = hit.target;
						hit.target.MovementComponent?.ApplyKnockback(hit.impactDirection * result.impulse);
					}
				}
			}

			NumUnresolvedHits = 0;

			return resolutionResults;
		}
	}
}
