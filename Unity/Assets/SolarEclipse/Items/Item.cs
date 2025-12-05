using UnityEngine;

using NaughtyAttributes;

namespace eclipse.items {
    [System.Serializable]
    public struct ItemDescription {
        [field:SerializeField]
        public string cDisplayName { get; private set; }
        [field:SerializeField, ShowAssetPreview]
        public Sprite cSprite {get; private set;}
    }

    [System.Serializable]
    public abstract class ItemBlueprint : ScriptableObject {
        [field:SerializeField]
        public ItemDescription cDescription {get; private set;}
    }

    [System.Serializable]
	public struct Item {
        public ItemBlueprint blueprint;
	}
}
