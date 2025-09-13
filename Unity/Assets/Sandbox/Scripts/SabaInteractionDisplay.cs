using UnityEngine;
using UnityEngine.UI;
using TMPro;

using NaughtyAttributes;
using PrettyPatterns;

namespace Saba {
	public struct InteractionDisplay {
		public Sprite sprite;
		public string prompt;
		public Vector3 location;
	}

	[RequireComponent(typeof(RectTransform))]
	public class SabaInteractionDisplay : SingletonNullable<SabaInteractionDisplay> {
		[SerializeField]
		float offset;
		[SerializeField, Required]
		Image image;
		[SerializeField, Required]
		TMP_Text prompt;

		RectTransform rectTransform;
		Canvas canvas;

		public override void Awake() {
			base.Awake();
			rectTransform = GetComponent<RectTransform>();
			canvas = GetComponentInParent<Canvas>();
		}

        void Start() {
            Hide();
        }

		public void UpdateData(InteractionDisplay data) {
			image.sprite = data.sprite;
			prompt.text = data.prompt;
			Vector2 viewportPosition = canvas.worldCamera.WorldToViewportPoint(data.location);
			Vector2 screenPosition = new Vector2(
				((viewportPosition.x * rectTransform.sizeDelta.x) - (rectTransform.sizeDelta.x * 0.5f)),
				((viewportPosition.y * rectTransform.sizeDelta.y) - (rectTransform.sizeDelta.y * 0.5f))
			);
            rectTransform.anchoredPosition = screenPosition + offset * Vector2.up;
			gameObject.SetActive(true);
		}

		public void Hide() { gameObject.SetActive(false); }
	}
}
