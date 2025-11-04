using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using PrettyPatterns;

namespace eclipse.ui {
	[RequireComponent(typeof(RectTransform))]
	public class HealthDisplayManager : Singleton<HealthDisplayManager> {
		const int MAX_BARS = 100;

		class Data {
			public int num = 0;
			public float[] cachedHealthToMaxHealthRatio;
			public Entity[] entities;
			public Slider[] sliders;
		}

		[SerializeField]
		Slider prefab;
		[SerializeField]
		Vector3 offset;

		new Camera camera;
		Data data;
		ObjectPool<Slider> healthBars;
		Dictionary<Entity, int> indexMap = new Dictionary<Entity, int>();

		public override void Awake() {
			base.Awake();

			data = new Data(
			) { num = 0,
				cachedHealthToMaxHealthRatio = new float[MAX_BARS],
				entities = new Entity[MAX_BARS],
				sliders = new Slider[MAX_BARS] };

			healthBars = new ObjectPool<Slider>(
				createFunc: () => Instantiate(prefab, transform),
				actionOnGet: healthBar => healthBar.gameObject.SetActive(true),
				actionOnRelease: healthBar => healthBar.gameObject.SetActive(false),
				actionOnDestroy: healthBar => Destroy(healthBar.gameObject),
				collectionCheck: false,
				defaultCapacity: 10,
				maxSize: MAX_BARS
			);
			Canvas parentCanvas = GetComponentInParent<Canvas>();
			Assert.IsNotNull(parentCanvas, "Expect this to be parented under a canvas");
			camera = parentCanvas.worldCamera;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void UpdatePosition(int i) {
			Vector3 position = data.entities[i].transform.position + offset.x * camera.transform.right +
							   offset.y * camera.transform.up + offset.z * camera.transform.forward;
			data.sliders[i].transform.position = RectTransformUtility.WorldToScreenPoint(camera, position);
		}

		public void Track(ReadOnlySpan<Entity> entities) {
			foreach (Entity entity in entities) {
				bool isAlreadyTracking = indexMap.ContainsKey(entity);

				float healthValue = Alias.health[entity.resource] / Alias.maxHealth[entity.stats];
				if (isAlreadyTracking) {
					int index = indexMap[entity];
					data.cachedHealthToMaxHealthRatio[index] = healthValue;
					data.sliders[index].value = healthValue;
				} else {
					Slider newBar = healthBars.Get();
					newBar.value = healthValue;

					indexMap[entity] = data.num;
					data.cachedHealthToMaxHealthRatio[data.num] = healthValue;
					data.sliders[data.num] = newBar;
					data.entities[data.num] = entity;
					UpdatePosition(data.num);
					data.num++;
				}
			}
		}

		public void Untrack(ReadOnlySpan<Entity> entities) {
			foreach (Entity entity in entities) {
				if (!indexMap.ContainsKey(entity)) continue;

				int index = indexMap[entity];
				int lastIndex = data.num - 1;

				healthBars.Release(data.sliders[index]);

				data.cachedHealthToMaxHealthRatio[index] = data.cachedHealthToMaxHealthRatio[lastIndex];
				data.sliders[index] = data.sliders[lastIndex];
				data.entities[index] = data.entities[lastIndex];

				indexMap[data.entities[index]] = index;

				data.sliders[lastIndex] = null;
				data.entities[lastIndex] = null;

				indexMap.Remove(entity);
				data.num--;
			}
		}

		public void OnDamageTaken(ReadOnlySpan<Entity> damaged) => Track(damaged);
		public void OnDeath(ReadOnlySpan<Entity> dead) => Untrack(dead);

		public void Run() {
			Assert.AreEqual(
				data.num, indexMap.Count, "Expect number of entities to match number of active health bars"
			);

			for (int i = 0; i < data.num; i++) UpdatePosition(i);
		}
	}
}
