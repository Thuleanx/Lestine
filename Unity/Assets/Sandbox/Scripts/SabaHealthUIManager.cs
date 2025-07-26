using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.Pool;
using System.Collections.Generic;

using PrettyPatterns;

namespace Saba {
    [RequireComponent(typeof(RectTransform))]
	public class SabaHealthUIManager : Singleton<SabaHealthUIManager> {
		const int MIN_BARS = 5;
		const int MAX_BARS = 100;

		struct TrackedEntityData {
			public Slider healthBar;
			public SabaEntity entity;
		};

		[SerializeField]
		Slider uiPrefab;
        [SerializeField]
        Vector3 offset;

		ObjectPool<Slider> healthBars;
        new Camera camera;

		List<TrackedEntityData> activeEntities = new List<TrackedEntityData>();
		Dictionary<SabaEntity, int> activeEntityIndexMap =
			new Dictionary<SabaEntity, int>();

		public override void Awake() {
			base.Awake();

			healthBars = new ObjectPool<Slider>(
				createFunc: () => Instantiate(uiPrefab, transform),
				actionOnGet: healthBar => healthBar.gameObject.SetActive(true),
				actionOnRelease: healthBar =>
					healthBar.gameObject.SetActive(false),
				actionOnDestroy: healthBar => Destroy(healthBar.gameObject),
				collectionCheck: false,
				defaultCapacity: MIN_BARS,
				maxSize: MAX_BARS
			);
            camera = GetComponentInParent<Canvas>().worldCamera;
		}

        public void Track(SabaEntity entity) => Track(new SabaEntity[]{entity});
        public void Untrack(SabaEntity entity) => Untrack(new SabaEntity[]{entity});

		void Track(IEnumerable<SabaEntity> entities) {
			foreach (SabaEntity entity in entities) {
				bool isAlreadyTracking =
					activeEntityIndexMap.ContainsKey(entity);
				if (isAlreadyTracking) continue;
				Slider newBar = healthBars.Get();

				activeEntityIndexMap[entity] = activeEntities.Count;
				activeEntities.Add(new TrackedEntityData(
				) { healthBar = newBar, entity = entity });
			}
		}

		void Untrack(IEnumerable<SabaEntity> entities) {
			foreach (SabaEntity entity in entities) {
				if (!activeEntityIndexMap.ContainsKey(entity)) continue;
				int index = activeEntityIndexMap[entity];
				int lastIndex = activeEntities.Count - 1;

				healthBars.Release(activeEntities[index].healthBar);

				activeEntities[index] = activeEntities[lastIndex];
				TrackedEntityData data = activeEntities[index];
				activeEntityIndexMap[data.entity] = index;

				activeEntities.RemoveAt(lastIndex);
				activeEntityIndexMap.Remove(entity);
			}
		}

		public void OnDamageTaken(IEnumerable<SabaEntity> damagedEntities
		) => Track(damagedEntities);
		public void OnDeath(IEnumerable<SabaEntity> deadEntities
		) => Untrack(deadEntities);

		void LateUpdate() {
			Assert.AreEqual(
				activeEntities.Count,
				activeEntityIndexMap.Count,
				"Expect number of entites to match number of active health bars"
			);

            foreach (TrackedEntityData data in activeEntities) {
                float healthValue = data.entity.Resource.Health /
                                    data.entity.Attributes.MaxHealth;
                data.healthBar.value = healthValue;

                Vector3 position = data.entity.transform.position + offset;
                data.healthBar.transform.position = RectTransformUtility.WorldToScreenPoint(camera, position);
            }
		}
	}
}
