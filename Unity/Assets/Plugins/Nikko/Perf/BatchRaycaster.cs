using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using System;

namespace Nikko.Perf {
    [BurstCompile]
    struct BatchRaycaster2DJob : IJobParallelFor {
        [ReadOnly]
        public NativeArray<RaycastCommand2D> requests;
        public NativeArray<RaycastHit2D> results;

        public void Execute(int index) {
            RaycastCommand2D command = requests[index];
            results[index] = Physics2D.Raycast(command.origin, command.direction, command.maxDistance, command.layerMask);
        }
    }

    struct RaycastCommand2D {
        public Vector2 origin;
        public Vector2 direction;
        public float maxDistance;
        public int layerMask;
    }

    public static class BatchRaycaster { 
		const int maxRaycastsPerJob = 10000;

		public static void PerformRaycasts(
			Vector2[] origins,
			Vector2[] directions,
			int layerMask,
			Action<RaycastHit2D[]> callback
		) {
			int rayCount = Mathf.Min(origins.Length, maxRaycastsPerJob);

		    NativeArray<RaycastCommand2D> rayCommands;
			using (
				rayCommands =
					new NativeArray<RaycastCommand2D>(rayCount, Allocator.TempJob)
			) {
				for (int i = 0; i < rayCount; i++) {
					rayCommands[i] = new RaycastCommand2D() {
                        origin = origins[i], 
                        direction = directions[i], 
                        maxDistance = directions[i].magnitude, 
                        layerMask = layerMask
                    };
				}

				ExecuteRaycasts(rayCommands, callback);
			}
		}

		static void ExecuteRaycasts(
			NativeArray<RaycastCommand2D> raycastCommands,
			Action<RaycastHit2D[]> callback
		) {
		    NativeArray<RaycastHit2D> hitResults;

			int maxHitsPerRaycast = 1;
			int totalHitsNeeded = raycastCommands.Length * maxHitsPerRaycast;

			using (
				hitResults = new NativeArray<RaycastHit2D>(
					totalHitsNeeded, Allocator.TempJob
				)
			) {
				foreach (RaycastCommand2D t in raycastCommands) {
					Debug.DrawLine(
						t.origin, t.origin + t.direction * 1f, Color.red, 0.1f
					);
				}

                // 2D async raycasts are still not supported, 
                // maybe we switch to Godot if the costs are too high
                // not that godot runs faster it's just less bloated
                
                // BatchRaycaster2DJob job = new BatchRaycaster2DJob() {
                //     requests = raycastCommands,
                //     results = hitResults
                // };
                // JobHandle raycastJobHandle = job.Schedule(totalHitsNeeded, 1);
				// raycastJobHandle.Complete();

                for (int i = 0; i < raycastCommands.Length; i++) {
                    RaycastCommand2D command = raycastCommands[i];
                    hitResults[i] = Physics2D.Raycast(command.origin, command.direction, command.maxDistance, command.layerMask);
                }

				if (hitResults.Length > 0) {
					RaycastHit2D[] results = hitResults.ToArray();

					// for (int i = 0; i < results.Length; i++) {
					//     if (results[i].collider != null) {
					//         Debug.Log($"Hit: {results[i].collider.name} at
					//         {results[i].point}");
					//         Debug.DrawLine(raycastCommands[i].from,
					//         results[i].point, Color.green, 1.0f);
					//     }
					// }

					callback?.Invoke(results);
				}
			}
		}
	}
}
