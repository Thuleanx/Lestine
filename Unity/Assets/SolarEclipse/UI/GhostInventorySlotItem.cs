using UnityEngine;
using UnityEngine.UI;

using PrettyPatterns;

namespace eclipse.ui {
    [RequireComponent(typeof(Image))]
	public class GhostInventorySlotItem : Singleton<GhostInventorySlotItem> {
        public Image cSprite {get; private set; }

        public override void Awake() {
            base.Awake();
            cSprite = GetComponent<Image>();
            cSprite.raycastTarget = false;

            Canvas canvas = GetComponentInParent<Canvas>();
            transform.SetParent(canvas.transform);
            transform.SetAsLastSibling();
        }

        void Start() {
            gameObject.SetActive(false);
        }
	}
}
