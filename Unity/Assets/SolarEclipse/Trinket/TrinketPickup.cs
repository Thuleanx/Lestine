using UnityEngine;

using eclipse.interactable;

namespace eclipse.trinket {
    public class TrinketPickup : Interactable {
        [SerializeField]
        Trinket trinket;

        public override string GetInteractionPrompt() => "Pickup: " + trinket.displayName;
        public override Sprite GetInteractionSprite() => trinket.sprite;

        public override void Interact(InteractionSource source) {
            TrinketContainer container = source.GetComponent<TrinketContainer>();
            if (container) container.Acquire(trinket);
            gameObject.SetActive(false);
        }
    }
}
