using UnityEngine;
using System.Collections.Generic;

using NaughtyAttributes;

using MathUtils;

namespace Saba {
	public class SabaDirector : MonoBehaviour {
		[SerializeField]
		float creditsOnActivation;
		[SerializeField]
		float creditMultiplier;
		[SerializeField, MinMaxSlider(0.01f, 10.0f)]
		Vector2 spawnInterval = Vector2.one;
		[SerializeField, MinMaxSlider(0.0f, 30.0f)]
		Vector2 spawnRange = Vector2.one;
		[SerializeField]
		SabaEnemyDataTable dataTable;

		[SerializeField]
		int maxSpawnsPerWave = 30;
		[SerializeField]
		SabaEntityBudget.Bucket spawnBucket = SabaEntityBudget.Bucket.Stage;

		new Camera camera;

		float credits;
		float timeNextWave;

		void OnEnable() {
			credits = creditsOnActivation;
			camera = Camera.main;
		}

		void SpawnWave() {
			if (SabaSpawnManager.instance.IsBucketFull(spawnBucket)) return;
			Vector2 spawnCenter = SabaPlayer.instance ? SabaPlayer.instance.transform.position : Vector2.zero;

			bool canAffordAny = false;
			float totalWeights = 0;
			// this is initialized high so that we recompute it
			// on the first span pass
			float maxAffordableEntry = float.MaxValue;

			List<SabaSpawnManager.SpawnParameter> spawnParameters =
				new List<SabaSpawnManager.SpawnParameter>(maxSpawnsPerWave);

			for (int i = 0; i < maxSpawnsPerWave; i++) {
				if (credits < maxAffordableEntry) {
					maxAffordableEntry = totalWeights = 0;
					canAffordAny = false;
					foreach (SabaEnemyDataTable.SpawnCard entry in dataTable.entries) {
						bool canAffordEntry = entry.cost <= credits;
						canAffordAny |= canAffordEntry;
						if (!canAffordEntry) continue;

						maxAffordableEntry = Mathf.Max(maxAffordableEntry, entry.cost);
						totalWeights += entry.weight;
					}
				}

				if (!canAffordAny) break;

				float spawnWeight = Mathx.RandomRange(0.0f, totalWeights);

				// choose spawn point
				float r = Mathx.RandomRange(spawnRange);
				float theta = Mathx.RandomRange(0, 2 * Mathf.PI);

				Vector2 spawnPosition = new Vector2(Mathf.Sin(theta), Mathf.Cos(theta)) * r + spawnCenter;

				foreach (SabaEnemyDataTable.SpawnCard entry in dataTable.entries) {
					bool canAffordEntry = entry.cost < credits;
					if (!canAffordEntry) continue;
					spawnWeight -= entry.weight;

					bool isChosenEntry = spawnWeight <= 0;
					if (isChosenEntry) {
						spawnParameters.Add(
							new SabaSpawnManager.SpawnParameter { position = spawnPosition, prefab = entry.prefab }
						);
						credits -= entry.cost;
						break;
					}
				}
			}

			SabaSpawnManager.instance.RequestSpawn(spawnBucket, spawnParameters);
		}

		void Update() {
			float deltaTime = Time.deltaTime;
			float creditGainRate = creditMultiplier;

			credits += deltaTime * creditGainRate;
			if (timeNextWave < Time.time) {
				SpawnWave();
				float waveCooldownSeconds = Mathx.RandomRange(spawnInterval);
				timeNextWave = Time.time + waveCooldownSeconds;
			}
		}
	}
}
