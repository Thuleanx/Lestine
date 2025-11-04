using UnityEngine;

namespace eclipse.interactable {
	public abstract class Interactable : MonoBehaviour {
        public abstract string GetInteractionPrompt();
        public abstract Sprite GetInteractionSprite();

        public virtual void Interact(InteractionSource source) {
            // does nothing
        }

        public void OnEnable() => InteractableSystem.instance.AddInteractable(this);
        public void OnDisable() => InteractableSystem.instance?.RemoveInteractable(this);
	}
}
