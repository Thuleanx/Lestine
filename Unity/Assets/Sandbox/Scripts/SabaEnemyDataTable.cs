using UnityEngine;

using NaughtyAttributes;

namespace Saba {
	[CreateAssetMenu(menuName = "Saba/EnemyDataTable")]
	public class SabaEnemyDataTable : ScriptableObject {
        [System.Serializable]
        public struct SpawnCard {
            public SabaEntity prefab;
            // Determines how frequent the randomness will choose said character
            [Min(0.0f)]
            public float weight;
            // Determines how much this character will cost to spawn, in credits
            [Min(0.0f)]
            public float cost;
        };

        [field:SerializeField, ReorderableList]
        public SpawnCard[] entries;
	}
}
