using UnityEngine;
using UnityEngine.Assertions;
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
		Vector3 trackingPosition;

		public override void Awake() {
			base.Awake();
			rectTransform = GetComponent<RectTransform>();
			canvas = GetComponentInParent<Canvas>();
		}

		void Start() { Hide(); }

		public void UpdateData(InteractionDisplay data) {
			image.sprite = data.sprite;
			prompt.text = data.prompt;
			trackingPosition = data.location;
			Reposition();

			gameObject.SetActive(true);
		}

		public void Hide() { gameObject.SetActive(false); }

		void Reposition() {
			Vector2 viewportPos =
				canvas.worldCamera.WorldToViewportPoint(trackingPosition + offset * canvas.worldCamera.transform.up);
			rectTransform.anchoredPosition = canvas.GetComponent<CanvasScaler>().referenceResolution * viewportPos;
		}

		void Update() => Reposition();
	}
}
