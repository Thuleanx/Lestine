using System;
using System.Collections.Generic;
using UnityEngine;

using PrettyPatterns;

namespace Saba {
	public class SabaSpawnManager : Singleton<SabaSpawnManager> {
		public struct SpawnParameter {
			public Vector2 position;
			public SabaEntity prefab;
		}

		public class Bucket {
			public int capacity;
			public int currentNumber;
			public int pendingNumber;
			public SabaEntity[] entities;
		}
		Bucket[] buckets;
		int sumBucketSize;

		List<SpawnParameter> pendingSpawns;
		List<SabaEntityBudget.Bucket> pendingSpawnBuckets;

		public override void Awake() {
			base.Awake();

			const int MAX_BUCKET = (int)SabaEntityBudget.Bucket.MAX;

			SabaEntityBudget budget = SabaSettings.Get().entityBudget;

			buckets = new Bucket[MAX_BUCKET + 1];
			sumBucketSize = 0;
			for (int i = 0; i <= MAX_BUCKET; i++) {
				int bucketSize = budget.GetCount((SabaEntityBudget.Bucket)i);
				buckets[i] =
					new Bucket() { capacity = bucketSize, currentNumber = 0, entities = new SabaEntity[bucketSize] };
			}

			pendingSpawns = new List<SpawnParameter>();
			pendingSpawnBuckets = new List<SabaEntityBudget.Bucket>();
		}

		void Update() => PerformSpawn();

		void PerformSpawn() {
			float frameBudgetMilliseconds = SabaSettings.Get().frameBudget.spawnMiliseconds;

			float currentTime = Time.unscaledTime;
			while (pendingSpawns.Count > 0) {
				int index = pendingSpawns.Count - 1;
				SabaEntityBudget.Bucket pendingBucket = pendingSpawnBuckets[index];
				SpawnParameter pendingParameter = pendingSpawns[index];

				Bucket bucket = buckets[(int)pendingBucket];

				bucket.entities[bucket.currentNumber++] =
					Instantiate(pendingParameter.prefab, pendingParameter.position, Quaternion.identity);
				bucket.pendingNumber--;

				pendingSpawns.RemoveAt(index);
				pendingSpawnBuckets.RemoveAt(index);

				float timeAfterSpawn = Time.unscaledTime;
				const float SECONDS_TO_MILI = 1000.0f;

				bool isOverTimeBudget = (timeAfterSpawn - currentTime) * SECONDS_TO_MILI > frameBudgetMilliseconds;
				if (isOverTimeBudget) break;
			}
		}

		public bool IsBucketFull(SabaEntityBudget.Bucket bucket) {
			Bucket bucketData = buckets[(int)bucket];
			return bucketData.currentNumber + bucketData.pendingNumber >= bucketData.capacity;
		}

		public void RequestSpawn(SabaEntityBudget.Bucket bucket, IEnumerable<SpawnParameter> spawnParameters) {
			int bucketIndex = (int)bucket;
			// should be a reference
			Bucket bucketData = buckets[bucketIndex];
			foreach (SpawnParameter spawnParam in spawnParameters) {
				if (bucketData.currentNumber + bucketData.pendingNumber >= bucketData.capacity) {
					Debug.LogWarning(
						"Requested spawn entity " + spawnParam.prefab + " but bucket " +
						Enum.GetName(typeof(SabaEntityBudget.Bucket), bucket) + " has reached capacity of " +
						bucketData.capacity + ". Stopping additional spawns"
					);
					break;
				}

				pendingSpawnBuckets.Add(bucket);
				pendingSpawns.Add(spawnParam);
				bucketData.pendingNumber++;
				sumBucketSize++;
			}
		}
	}
}
