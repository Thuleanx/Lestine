using UnityEngine;

using eclipse.interactable;

namespace eclipse.trinket {
	public class TrinketPickup : Interactable {
		[SerializeField]
		Trinket trinket;

		public override string GetInteractionPrompt() => "Pickup: " + trinket.cDescription.cDisplayName;
		public override Sprite GetInteractionSprite() => trinket.cDescription.cSprite;

		public override void Interact(InteractionSource source) {
			TrinketContainer container = source.GetComponent<TrinketContainer>();
			if (container) container.Acquire(trinket);
			else {
				Debug.LogError(
					"Tried to interact with trinket pickup without a trinket container: " + source.gameObject
				);
			}
			gameObject.SetActive(false);
		}
	}
}
