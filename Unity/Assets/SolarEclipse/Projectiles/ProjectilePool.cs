using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Assertions;

using NaughtyAttributes;

using Nikko.Perf;

namespace eclipse.projectile {
	public class ProjectilePool : MonoBehaviour {
		const int MAX_BULLETS = 10000;

		[SerializeField, MinMaxSlider(0, MAX_BULLETS)]
		Vector2Int bulletCapacity;

		[SerializeField, ShowAssetPreview]
		GameObject bulletPrefab;
		[SerializeField]
		float speed;
		[SerializeField]
		float lifetime;
		[SerializeField]
		LayerMask layerMask;

		ObjectPool<GameObject> pool;

		void Awake() {
			pool = new ObjectPool<GameObject>(
                createFunc: () => {
                    GameObject newBullet = Instantiate(bulletPrefab);
                    newBullet.SetActive(true);
                    return newBullet;
                },
                actionOnGet: bullet => bullet.gameObject.SetActive(true),
                actionOnRelease: bullet => bullet.gameObject.SetActive(false),
                actionOnDestroy: bullet => Destroy(bullet.gameObject),
                collectionCheck: false,
                defaultCapacity: bulletCapacity.x,
                maxSize: bulletCapacity.y
            );
		}

		public void InstantiateBullet(Entity owner, Vector2 source, Vector2 direction, float timeTravelled) {
			GameObject bullet = pool.Get();
			ProjectileSystem.instance.Add(bullet, this, owner, source, direction * speed, layerMask, lifetime, timeTravelled);
		}

		public void Release(GameObject obj) => pool.Release(obj);
	}
}
