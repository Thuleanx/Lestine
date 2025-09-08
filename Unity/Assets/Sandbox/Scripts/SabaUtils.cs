using UnityEngine;
using System.Collections.Generic;

namespace Saba {
	public static class Utils {
		public static T GetClosest<T>(IEnumerable<T> positions, Vector2 target)
			where T : MonoBehaviour {
            T closest = null;
            float closestDistanceSq = float.MaxValue;

            foreach (T candidate in positions) {
                Vector2 candidatePosition = candidate.transform.position;
                float distanceSq = (target - candidatePosition).sqrMagnitude;

                if (closestDistanceSq > distanceSq) {
                    closestDistanceSq = distanceSq;
                    closest = candidate;
                }
            }

            return closest;
        }
	}
}
