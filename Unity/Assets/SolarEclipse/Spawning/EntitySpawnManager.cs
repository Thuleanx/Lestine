using UnityEngine;

using PrettyPatterns;

namespace eclipse.spawning {
	public class EntitySpawnManager : Singleton<EntitySpawnManager> {
		public class SpawnParameters {
			public const int MAX_SPAWNS = 1000;

			public Entity[] prefab;
			public Vector2[] position;
			// Core stats
			public float[] health;
			public float[] defense;
			public float[] damageReduction;
			public float[] movementSpeed;

			public int current = 0;
			public int num = 0;

			public static SpawnParameters Create() {
				return new SpawnParameters(
				) { prefab = new Entity[MAX_SPAWNS],
					position = new Vector2[MAX_SPAWNS],
					health = new float[MAX_SPAWNS],
					defense = new float[MAX_SPAWNS],
					damageReduction = new float[MAX_SPAWNS],
					movementSpeed = new float[MAX_SPAWNS] };
			}
		}
		SpawnParameters pending = SpawnParameters.Create();

		const int MAX_SPAWNS_PER_TICK = 30;

        public struct SpawnRequest {
            public Entity prefab;
            public Vector2 position;
            public float health;
            public float defense;
			public float damageReduction;
			public float movementSpeed;
        }
        public void RequestSpawn(SpawnRequest request) {
            int index = (pending.current + pending.num);
            if (index > SpawnParameters.MAX_SPAWNS) index -= SpawnParameters.MAX_SPAWNS;

            pending.prefab[index] = request.prefab;
            pending.position[index] = request.position;
            pending.health[index] = request.health;
            pending.defense[index] = request.defense;
            pending.damageReduction[index] = request.damageReduction;
            pending.movementSpeed[index] = request.movementSpeed;

            pending.num++;
        }

		public void Run() {
			if (pending.num == 0) return;

			Entity[] spawnedEntities = new Entity[MAX_SPAWNS_PER_TICK];

			int i = 0;
			for (; i < MAX_SPAWNS_PER_TICK && pending.num > i; i++) {
				int index = (pending.current + i) % SpawnParameters.MAX_SPAWNS;

				spawnedEntities[i] = Instantiate(pending.prefab[index], pending.position[index], Quaternion.identity);
			}

			if (i == 0) return;

			int stats = (eclipse.Alias.coreStats as RemovableSpanList).Allocate(i);
			int resource = (eclipse.Alias.coreResource as RemovableSpanList).Allocate(i);

			for (int j = 0; j < i; j++) {
				spawnedEntities[j].stats = stats + j;
				spawnedEntities[j].resource = resource + j;

				int pendingIndex = (pending.current + j) % SpawnParameters.MAX_SPAWNS;

				Alias.maxHealth[stats + j] = Alias.health[resource + j] = pending.health[pendingIndex];
				Alias.defense[stats + j] = pending.defense[pendingIndex];
				Alias.damageReduction[stats + j] = pending.damageReduction[pendingIndex];
				Alias.movementSpeed[stats + j] = pending.movementSpeed[pendingIndex];
                Alias.coreStats.entities[stats + j] = spawnedEntities[j];
			}

			pending.current = (pending.current + i) % SpawnParameters.MAX_SPAWNS;
			pending.num -= i;
		}
	}
}
