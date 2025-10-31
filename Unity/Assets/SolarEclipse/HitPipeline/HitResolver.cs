using UnityEngine;
using System.Runtime.CompilerServices;

using PrettyPatterns;

namespace eclipse.hit {
	public class HitResolver : Singleton<HitResolver> {
		const int MAX_HIT = 10000;

		int num;
		Hit[] unresolved = new Hit[MAX_HIT];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(Hit hit) { unresolved[num++] = hit; }

		public struct ResolutionResult {
			// Everyone in indicex [0, numDead) are dead
			// and [numDead, numHit + numDead) are just damaged
            public int numDead;
            public int numHit;
            public Entity[] entities;
        }

		public ResolutionResult Run() {
			ResolutionResult result = new ResolutionResult() {
                numDead = 0,
                numHit = 0,
                entities = null
            };

			if (num == 0) return result;
            result.entities = new Entity[num];

			PostMitigatedHit[] postMitigatedHits = DamageResolver.Process(new System.Span<Hit>(unresolved, 0, num));
			for (int i = 0; i < num; i++) {
                Hit hit = unresolved[i];
                if (postMitigatedHits[i].type == PostMitigatedHit.Type.Hit) {
                    bool isKillingBlow = !EntityStatics.IsDead(hit.target);
                    Alias.health[hit.target.stats] -= postMitigatedHits[i].damage;
                    isKillingBlow &= EntityStatics.IsDead(hit.target);

                    if (isKillingBlow) {
                        result.entities[result.numHit + result.numDead] = 
                            result.entities[result.numDead];
                        result.entities[result.numDead++] = unresolved[i].target;
                    } else {
                        result.entities[(result.numHit++) + result.numDead] = unresolved[i].target;
                    }
                }
            }

            num = 0;

			return result;
		}
	}
}
