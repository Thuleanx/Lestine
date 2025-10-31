using UnityEngine;
using System;

using eclipse.hit;
using eclipse.spawning;
using eclipse.ui;

namespace eclipse.scheduling {
	public class Scheduler : MonoBehaviour {
		void Update() {
			EntitySpawnManager.instance.Run();

			HitResolver.ResolutionResult result = HitResolver.instance.Run();

			ReadOnlySpan<Entity> damaged = result.numHit > 0
				? new ReadOnlySpan<Entity>(result.entities, result.numDead, result.numDead + result.numHit)
				  : ReadOnlySpan<Entity>.Empty;

			HealthDisplayManager.instance.OnDamageTaken(damaged);
		}
	}
}
