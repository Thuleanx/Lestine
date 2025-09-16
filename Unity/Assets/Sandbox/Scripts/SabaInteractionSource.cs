using UnityEngine;

using ADammy;

namespace Saba {
	[RequireComponent(typeof(SabaEntity))]
	public class SabaInteractionSource : MonoBehaviour {
		[SerializeField]
		float range;

		SabaEntity entity;

		EventBinding<InteractionAction> interactActionBinding;

		void Awake() {
			entity = GetComponent<SabaEntity>();
			interactActionBinding = new EventBinding<InteractionAction>(Interact);
		}

		void OnEnable() => interactActionBinding.Bind();
		void OnDisable() => interactActionBinding.Unbind();

		SabaInteractable lastClosestInteractable;

		void Update() {
			SabaInteractable closestInteractable =
				Utils.GetClosest(AllSabaInteractables.instance.AsList, transform.position);
			if (closestInteractable) {
				Vector2 displacement = closestInteractable.transform.position - transform.position;
				bool isInteractableClose = displacement.sqrMagnitude < range * range;
				if (!isInteractableClose) closestInteractable = null;

				if (lastClosestInteractable != closestInteractable) {
					if (closestInteractable == null) SabaInteractionDisplay.instance?.Hide();
					else
						SabaInteractionDisplay.instance.UpdateData(new InteractionDisplay(
						) { sprite = closestInteractable.GetInteractionSprite(),
							prompt = closestInteractable.GetInteractionPrompt(),
							location = closestInteractable.transform.position });
				}
			}

			lastClosestInteractable = closestInteractable;
		}

		public void Interact() {
			SabaInteractable interactable = Utils.GetClosest(AllSabaInteractables.instance.AsList, transform.position);
			if (interactable) {
				Vector2 displacement = interactable.transform.position - transform.position;
				bool isInteractableClose = displacement.sqrMagnitude < range * range;

				if (isInteractableClose) interactable.Interact(entity);
			}
		}
	}
}
