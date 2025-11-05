using UnityEngine;

using NaughtyAttributes;
using eclipse.items;

namespace eclipse.trinket {
	[CreateAssetMenu(fileName = "Trinket", menuName = "eclipse/trinket", order = 1)]
	public class Trinket : Item {
        [ShowAssetPreview]
        public Sprite sprite;
        public string displayName;
    }
}
