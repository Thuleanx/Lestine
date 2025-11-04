using UnityEngine;

using PrettyPatterns;
using Nikko.Perf;

namespace eclipse.projectile {
	public class ProjectileSystem : Singleton<ProjectileSystem> {
		const int MAX_PROJECTILES = 10000;

		public class ProjectileData {
			public int num;
			public ProjectilePool[] pools;
			public Entity[] owners;
			public GameObject[] bullets;
			public Vector2[] startPosition;
			public Vector2[] startVelocity;
			public float[] startTime;
			public float[] lifetime;
			public int[] layerMasks;

			public static ProjectileData Create() {
				return new ProjectileData {
					pools = new ProjectilePool[MAX_PROJECTILES],
					owners = new Entity[MAX_PROJECTILES],
					bullets = new GameObject[MAX_PROJECTILES],
					startPosition = new Vector2[MAX_PROJECTILES],
					startVelocity = new Vector2[MAX_PROJECTILES],
					startTime = new float[MAX_PROJECTILES],
					lifetime = new float[MAX_PROJECTILES],
					layerMasks = new int[MAX_PROJECTILES]
				};
			}

			public Vector2 PositionAt(int bullet, float time) =>
				startVelocity[bullet] * (time - startTime[bullet]) + startPosition[bullet];
		};

		ProjectileData data;
		float lastSimulationTime;
		bool readyForSimulation;

		public override void Awake() {
			base.Awake();
			data = ProjectileData.Create();
		}

		public void Add(
			GameObject projectile,
			ProjectilePool pool,
			Entity owner,
			Vector2 start,
			Vector2 direction,
            int layerMask,
            float lifetime,
			float timeTraveled
		) {
            data.bullets[data.num] = projectile;
            data.pools[data.num] = pool;
            data.owners[data.num] = owner;
            data.startPosition[data.num] = start;
            data.startVelocity[data.num] = direction;
            data.layerMasks[data.num] = layerMask;
            data.lifetime[data.num] = lifetime;
            data.startTime[data.num] = Time.time - timeTraveled;

            projectile.transform.position = data.PositionAt(data.num, Time.time);
            data.num++;
        }

		void OnEnable() {
			readyForSimulation = true;
			lastSimulationTime = Time.time;
		}

		void Simulate() {
			if (data.num == 0) return;

			Vector2[] origins = new Vector2[data.num];
			Vector2[] directions = new Vector2[data.num];

			float currentTime = Time.time;

			// TODO: can be sped up with simd, i believe
			for (int i = 0; i < data.num; i++) {
				float traceStartTime = Mathf.Max(lastSimulationTime, data.startTime[i]);
				origins[i] = data.PositionAt(i, traceStartTime);
				Vector2 nextPosition = data.PositionAt(i, currentTime);
				directions[i] = nextPosition - origins[i];
			}

			lastSimulationTime = currentTime;
			readyForSimulation = false;
			BatchRaycaster.PerformRaycasts(origins, directions, data.layerMasks, OnRaycastComplete);
		}

		void OnRaycastComplete(RaycastHit2D[] hits) {
			// We quit the game or this object gets disabled in the middle of
			// a raycast request
			if (!enabled) return;

			for (int i = hits.Length - 1; i >= 0; i--) {
				RaycastHit2D hit = hits[i];

				float elapsedTime = Time.time - data.startTime[i];

				bool isExpired = elapsedTime > data.lifetime[i];
				float destroyedTime = data.lifetime[i] + data.startTime[i];

				bool isHit = hit.collider != null;
				if (!isHit && !isExpired) continue;

				if (isHit) {
					Vector2 hitDisplacement = (Vector2)hit.point - data.startPosition[i];
					float speed = data.startVelocity[i].magnitude;
					float distanceTravelled = hitDisplacement.magnitude;
					destroyedTime = Mathf.Min(destroyedTime, distanceTravelled / speed);

					Entity entity = hit.collider.GetComponentInParent<Entity>();
					if (entity && !EntityStatics.IsDead(entity) && data.owners[i]) {
						eclipse.hit.HitResolver.instance.Add(new hit.Hit(
						) { target = entity,
							attacker = data.owners[i],
							baseDamage = 1.0f,
							knockback = 1.0f,
							location = hit.point,
							direction = data.startVelocity[i].normalized });
					}
				}

				data.bullets[i].transform.position = data.PositionAt(i, destroyedTime);

                data.pools[i].Release(data.bullets[i]);
			    int lastIndex = data.num - 1;

                data.pools[i] = data.pools[lastIndex];
                data.owners[i] = data.owners[lastIndex];
                data.bullets[i] = data.bullets[lastIndex];
                data.startPosition[i] = data.startPosition[lastIndex];
                data.startVelocity[i] = data.startVelocity[lastIndex];
                data.startTime[i] = data.startTime[lastIndex];
                data.lifetime[i] = data.lifetime[lastIndex];
                data.layerMasks[i] = data.layerMasks[lastIndex];

                data.pools[lastIndex] = null;
                data.owners[lastIndex] = null;
                data.bullets[lastIndex] = null;

				data.num--;
			}

			readyForSimulation = true;
		}

        void Update() {
			if (readyForSimulation && data.num > 0) Simulate();
        }

        void FixedUpdate() {
            for (int i = 0; i < data.num; i++)
                data.bullets[i].transform.position = data.PositionAt(i, Time.time);
        }
	}
}
