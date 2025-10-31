using UnityEngine;
using System.Collections;

using PrettyPatterns;
using MathUtils;

namespace eclipse.director {
	public class Encounter : MonoBehaviour {
		[SerializeField]
		SpawnTable spawnTable;
		[SerializeField]
		float range;
		[SerializeField]
		int numberOfSpawns;

		void TrySpawn(SpawnCard card) {
			Vector2 spawnCenter = transform.position;
			Vector2 displacement = Random.insideUnitCircle * range;
			Vector2 spawnLocation = spawnCenter + displacement;

			eclipse.spawning.EntitySpawnManager.instance.RequestSpawn(new spawning.EntitySpawnManager.SpawnRequest() {
				prefab = card.prefab,
                position = spawnLocation,
				health = card.coreStats.maxHealth,
				defense = card.coreStats.defense,
				damageReduction = card.coreStats.damageReduction,
				movementSpeed = card.coreStats.movementSpeed,
			});
		}

		void Start() {
			float[] chosenWeights = new float[numberOfSpawns];
			for (int i = 0; i < numberOfSpawns; i++) chosenWeights[i] = Mathx.RandomRange(0, spawnTable.totalWeights);

			if (spawnTable.spawnCards.Length == 0) return;

			System.Array.Sort(chosenWeights);
			int tableEntry = 0;
			float cumulativeWeights = spawnTable.spawnCards[0].weight;
			for (int i = 0; i < numberOfSpawns; i++) {
				while (cumulativeWeights < chosenWeights[i] && tableEntry < spawnTable.spawnCards.Length)
					cumulativeWeights += spawnTable.spawnCards[++tableEntry].weight;
                TrySpawn(spawnTable.spawnCards[tableEntry]);
			}
		}
	}
}
