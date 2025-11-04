using UnityEngine;
using UnityEngine.UI;
using TMPro;

using ADammy;
using PrettyPatterns;
using NaughtyAttributes;

namespace eclipse.ui {
	[RequireComponent(typeof(RectTransform))]
	public class InteractionDisplay : SingletonNullable<InteractionDisplay> {
		[SerializeField]
		float offset;
		[SerializeField, Required]
		Image image;
		[SerializeField, Required]
		TMP_Text prompt;

		RectTransform rectTransform;
		Canvas canvas;
		Vector3 trackingPosition;

        EventBinding<FocusInteractableDrop> interactionDropBinding;
        EventBinding<FocusInteractableChange> interactionChangeBinding;

		public override void Awake() {
			base.Awake();
			rectTransform = GetComponent<RectTransform>();
			canvas = GetComponentInParent<Canvas>();

            interactionDropBinding = new EventBinding<FocusInteractableDrop>(Hide);
            interactionChangeBinding = new EventBinding<ui.FocusInteractableChange>(UpdateData);
		}

		void Start() { 
            Hide(default);

            interactionDropBinding.Bind();
            interactionChangeBinding.Bind();
        }

        void OnDestroy() {
            interactionDropBinding.Unbind();
            interactionChangeBinding.Unbind();
        }

        void Hide(FocusInteractableDrop data) {
            gameObject.SetActive(false);
        }

		void UpdateData(FocusInteractableChange data) {
			image.sprite = data.sprite;
			prompt.text = data.prompt;
			trackingPosition = data.location;
			Reposition();

			gameObject.SetActive(true);
		}

		void Reposition() {
			Vector2 viewportPos =
				canvas.worldCamera.WorldToViewportPoint(trackingPosition + offset * canvas.worldCamera.transform.up);
			rectTransform.anchoredPosition = canvas.GetComponent<CanvasScaler>().referenceResolution * viewportPos;
		}

		void Update() => Reposition();
	}
}
