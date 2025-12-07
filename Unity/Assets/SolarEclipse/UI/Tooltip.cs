using UnityEngine;
using UnityEngine.UI;
using TMPro;

using eclipse.input;

using PrettyPatterns;
using NaughtyAttributes;

namespace eclipse.ui {
	public class Tooltip : Singleton<Tooltip> {
		public Camera mainCam { get; private set; }

		[field:SerializeField, Required]
		public TMP_Text cTitle {
			get; private set;
		}
		[field:SerializeField, Required]
		public TMP_Text cDescription {
			get; private set;
		}
		[field:SerializeField, Required]
		public Image cImage {
			get; private set;
		}
		[field:SerializeField, Required]
		public RectTransform cRoot {
			get; private set;
		}

		public MonoBehaviour Owner { get; private set; }

		public struct Content {
			public string title;
			public string description;
			public Sprite image;
		}

		public void Show(Content content, MonoBehaviour owner) {
			mainCam = Camera.main;
			this.Owner = owner;
			cTitle.text = content.title;
			cDescription.text = content.description;
			cImage.gameObject.SetActive(content.image != null);
			cImage.sprite = content.image;
            Reposition();
			gameObject.SetActive(true);
		}

		public void Hide() {
			Owner = null;
			gameObject.SetActive(false);
		}

        void Reposition() {
			cRoot.position = PointerPosition.Value;
			Vector2 viewportPos = mainCam.WorldToViewportPoint(cRoot.position);

			cRoot.pivot = new Vector2(
                viewportPos.x < 0.5 ? 0 : 1, 
                viewportPos.y < 0.5 ? 0 : 1
            );
        }

		void Update() {
			// If owner despawns, and somehow doesn't give us
			if (!Owner) {
                Hide();
                return;
            }

            Reposition();
		}
	}
}
