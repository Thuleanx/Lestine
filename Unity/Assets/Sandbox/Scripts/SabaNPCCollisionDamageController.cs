using UnityEngine;
using System.Collections.Generic;

namespace Saba {
	public class SabaNPCCollisionDamageController : MonoBehaviour {
		const int MaxMemory = 100;

		[SerializeField]
		float playerRadius = 0.2f;
		[SerializeField]
		float memorySeconds;

		struct MemoryOfHit {
			public float time;
			public int entity;

			public bool IsExpired(float memorySeconds) => Time.time - time
														  > memorySeconds;
		};

		HashSet<int> allEntitesRemembered = new HashSet<int>();
		MemoryOfHit[] memory = new MemoryOfHit[MaxMemory];
		int lt = 0;
		int rt = 0;

		List<SabaNPCData> data => SabaNPCRuntimeGroup.instance.data;
        SabaNPCTransientData transientData => SabaNPCRuntimeGroup.instance.transientData;

		void Forget() {
			while (lt != rt && memory[lt].IsExpired(memorySeconds)) {
				allEntitesRemembered.Remove(memory[lt++].entity);
				if (lt == MaxMemory) lt = 0;
			}
		}

		void Update() {
			Forget();

			SabaPlayer player = SabaPlayer.instance;
			if (!player) return;

			List<SabaHitResolution.Hit> allHits =
				new List<SabaHitResolution.Hit>();

			for (int npcIndex = 0; npcIndex < data.Count; npcIndex++) {
				SabaNPCData npc = data[npcIndex];
				int id = npc.entity.gameObject.GetInstanceID();

				bool hasHitPlayerRecently = allEntitesRemembered.Contains(id);
				if (hasHitPlayerRecently) continue;

				transientData.hasHitPlayerRecently[npcIndex] = false;

				Vector2 displacementToPlayer =
					player.transform.position - npc.entity.transform.position;
				float distanceToPlayer = displacementToPlayer.magnitude;
				Vector2 directionToPlayer = displacementToPlayer.normalized;

				bool isCollidingWithPlayer =
					distanceToPlayer < playerRadius + npc.radius;

				if (!isCollidingWithPlayer) continue;

				transientData.hasHitPlayerRecently[npcIndex] = true;

				allEntitesRemembered.Add(id);
				memory[rt++] =
					new MemoryOfHit() { time = Time.time, entity = id };

				if (rt == MaxMemory) rt = 0;

				if (rt == lt) {
					Debug.LogError(
						"Collision hit register exceeded capacity, silently dropping oldest entry"
					);
					lt++;
				}

				allHits.Add(new SabaHitResolution.Hit(
				) { Entity = player.entity,
					MovementComponent = player.movementComponent,
					Location = directionToPlayer * npc.radius +
							   (Vector2)npc.entity.transform.position,
					Direction = directionToPlayer,
					Damage = 1,
					Knockback = 0.5f });
			}

			SabaHitResolution.instance?.RegisterHits(allHits);
		}
	}
}
