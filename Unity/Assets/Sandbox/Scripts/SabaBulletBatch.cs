using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Assertions;

using NaughtyAttributes;

using Nikko.Perf;

namespace Saba {
	public class SabaBulletBatch : MonoBehaviour {
		[SerializeField, MinMaxSlider(0, 10000)]
		Vector2Int bulletCapacity;

		[SerializeField, ShowAssetPreview]
		SabaBullet bulletPrefab;
        [SerializeField]
        float speed;
		[SerializeField]
		float lifetime;
		[SerializeField]
		LayerMask layerMask;

		ObjectPool<SabaBullet> pool;
		List<SabaBullet> liveBullets;
		bool readyForSimulation;
		float lastSimulationTime;

		void Awake() {
			pool = new ObjectPool<SabaBullet>(
                createFunc: () => {
                    SabaBullet newBullet = Instantiate(bulletPrefab);
                    newBullet.gameObject.SetActive(true);
                    return newBullet;
                },
                actionOnGet: bullet => bullet.gameObject.SetActive(true),
                actionOnRelease: bullet => bullet.gameObject.SetActive(false),
                actionOnDestroy: bullet => Destroy(bullet.gameObject),
                collectionCheck: false,
                defaultCapacity: bulletCapacity.x,
                maxSize: bulletCapacity.y
            );
			liveBullets = new List<SabaBullet>();
		}

		public void InstantiateBullet(
			Vector3 source, Vector3 direction
		) {
            direction.Normalize();

			SabaBullet bullet = pool.Get();
			bullet.Initialize(source, speed * direction);
			liveBullets.Add(bullet);
		}

		void OnEnable() {
			readyForSimulation = true;
			lastSimulationTime = Time.time;
		}

		void Update() {
			if (readyForSimulation && liveBullets.Count > 0) Simulate();
		}

		void LateUpdate() {
			foreach (SabaBullet bullet in liveBullets)
				bullet.transform.position = bullet.PositionAt(Time.time);
		}

		void Simulate() {
			// Simulate
			Vector3[] origins = new Vector3[liveBullets.Count];
			Vector3[] directions = new Vector3[liveBullets.Count];

			for (int i = 0; i < liveBullets.Count; i++) {
				SabaBullet bullet = liveBullets[i];
				origins[i] = bullet.PositionAt(lastSimulationTime);

				float elapsedTime = (Time.time - bullet.StartTime);
				Vector3 nextPosition =
					bullet.StartVelocity * elapsedTime + bullet.StartPosition;

				directions[i] = nextPosition - origins[i];
			}

			lastSimulationTime = Time.time;
			readyForSimulation = false;
			BatchRaycaster.PerformRaycasts(
				origins, directions, layerMask, RemoveCollidedBullets
			);
		}

		void RemoveCollidedBullets(RaycastHit[] hits) {
            // We quit the game or this object gets disabled in the middle of 
            // a raycast request
            if (!enabled) return;

			Assert.AreEqual(liveBullets.Count, hits.Length);

			int lastIndex = liveBullets.Count - 1;
			for (int i = lastIndex; i >= 0; i--) {
				float elapsedTime = Time.time - liveBullets[i].StartTime;

				bool isHit = hits[i].collider != null;
				bool isExpired = elapsedTime > lifetime;
				if (isHit || isExpired) {
					float destroyedTime = lifetime + liveBullets[i].StartTime;

					if (isHit) {
						Vector3 hitDisplacement =
							(hits[i].point - liveBullets[i].StartPosition);
						float speed = liveBullets[i].StartVelocity.magnitude;
						float distanceTravelled = hitDisplacement.magnitude;

                        Assert.IsTrue(speed > 0);

                        destroyedTime = Mathf.Min(
                            destroyedTime, distanceTravelled / speed
                        );
					}

					liveBullets[i].transform.position =
						liveBullets[i].PositionAt(destroyedTime);

					// swap back
					SabaBullet tmp = liveBullets[i];
					pool.Release(tmp);

					liveBullets[i] = liveBullets[lastIndex];
					liveBullets[lastIndex] = tmp;

					lastIndex--;
				}
			}

			if (lastIndex + 1 < liveBullets.Count) {
				int startRemoveIndex = lastIndex + 1;
				int numToRemove = liveBullets.Count - startRemoveIndex;

				liveBullets.RemoveRange(startRemoveIndex, numToRemove);
			}

			readyForSimulation = true;
		}
	}
}
