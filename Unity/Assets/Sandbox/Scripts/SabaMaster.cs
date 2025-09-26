using UnityEngine;
using System;

namespace Saba {
	public class SabaMaster : MonoBehaviour {
		void Update() {
			SabaHitResolution.ResolutionResult result = SabaHitResolution.instance.Resolve();

			ReadOnlySpan<SabaEntity> dead = result.numDead > 0
												? new ReadOnlySpan<SabaEntity>(result.allHitEntities, 0, result.numDead)
												: ReadOnlySpan<SabaEntity>.Empty;
			ReadOnlySpan<SabaEntity> damaged =
				result.numDead < result.allHitEntities.Length
					? new ReadOnlySpan<SabaEntity>(result.allHitEntities, result.numDead, result.allHitEntities.Length)
					: ReadOnlySpan<SabaEntity>.Empty;

			SabaHealthUIManager.instance.OnDamageTaken(damaged);
			SabaHealthUIManager.instance.OnDeath(dead);
			if (!dead.IsEmpty) { SabaEntity.Kill(dead); }
		}
	}
}
