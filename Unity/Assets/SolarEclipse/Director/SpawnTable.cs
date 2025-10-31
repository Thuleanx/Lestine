using UnityEngine;

using NaughtyAttributes;

namespace eclipse.director {
	[CreateAssetMenu(menuName = "eclipse/spawnTable")]
	public class SpawnTable : ScriptableObject {
		public SpawnCard[] spawnCards;
		[ReadOnly]
		public float[] cumulativeWeights;
		[ReadOnly]
		public float totalWeights;

		public void OnValidate() {
			totalWeights = 0;
			cumulativeWeights = new float[spawnCards.Length];
			for (int i = 0; i < spawnCards.Length; i++) {
				totalWeights += spawnCards[i].weight;
				cumulativeWeights[i] = spawnCards[i].weight + 
                    (i > 0 ? cumulativeWeights[i - 1] : 0);
			}
		}
	}
}
