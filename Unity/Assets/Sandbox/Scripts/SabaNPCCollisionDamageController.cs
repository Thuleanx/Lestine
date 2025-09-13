using UnityEngine;
using System.Collections.Generic;

namespace Saba {
	public class SabaNPCCollisionDamageController : MonoBehaviour {
		const int MaxMemory = 100;

		[SerializeField]
		float playerRadius = 0.2f;
		[SerializeField]
		float knockbackVelocityScale = 0.3f;

		[SerializeField]
		float memorySeconds;

		struct MemoryOfHit {
			public float time;
			public int entity;

			public bool IsExpired(float memorySeconds) => Time.time - time > memorySeconds;
		};

		HashSet<int> allEntitesRemembered = new HashSet<int>();
		Deque<MemoryOfHit> memory = new Deque<MemoryOfHit>(MaxMemory);

		List<SabaNPCData> data => SabaNPCRuntimeGroup.instance.data;
		SabaNPCTransientData transientData => SabaNPCRuntimeGroup.instance.transientData;

		void Forget() {
			while (!memory.IsEmpty && memory.Peek().IsExpired(memorySeconds)) {
				allEntitesRemembered.Remove(memory.Peek().entity);
				memory.Pop();
			}
		}

		void Update() {
			Forget();

			SabaPlayer player = SabaPlayer.instance;
			if (!player) return;

			List<SabaHitResolution.Hit> allHits = new List<SabaHitResolution.Hit>();

			for (int npcIndex = 0; npcIndex < data.Count; npcIndex++) {
				SabaNPCData npc = data[npcIndex];
				int id = npc.entity.gameObject.GetInstanceID();

				bool hasHitPlayerRecently = allEntitesRemembered.Contains(id);
				if (hasHitPlayerRecently) continue;

				transientData.hasHitPlayerRecently[npcIndex] = false;

				Vector2 displacementToPlayer = player.transform.position - npc.entity.transform.position;
				float distanceToPlayer = displacementToPlayer.magnitude;
				Vector2 directionToPlayer = displacementToPlayer.normalized;

				bool isCollidingWithPlayer = distanceToPlayer < playerRadius + npc.radius;

				if (!isCollidingWithPlayer) continue;

				transientData.hasHitPlayerRecently[npcIndex] = true;

				allEntitesRemembered.Add(id);
				memory.Push(new MemoryOfHit() { time = Time.time, entity = id });

				Vector2 movementDirection = npc.movementComponent.Velocity.normalized;
				float speed = npc.movementComponent.Velocity.magnitude;

				float knockback = speed * knockbackVelocityScale;
				Vector2 location = directionToPlayer * npc.radius + (Vector2)npc.entity.transform.position;

				allHits.Add(new SabaHitResolution.Hit(
				) { Entity = player.entity,
					MovementComponent = player.movementComponent,
					Location = location,
					Direction = movementDirection,
					Damage = 1,
					Knockback = knockback });
			}

			SabaHitResolution.instance?.RegisterHits(allHits);
		}
	}
}
