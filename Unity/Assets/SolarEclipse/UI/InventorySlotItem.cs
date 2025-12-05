using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace eclipse.ui {
	[RequireComponent(typeof(Image))]
	public class InventorySlotItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler {
        [field:SerializeField]
        public Color cDraggingColor {get; private set;}
        Color droppedColor;

		public InventorySlot cParentSlot { get; private set; }
		public Image cSprite { get; private set; }

        public static InventorySlotItem draggedItem {get; private set;}

		void Awake() {
			cParentSlot = GetComponentInParent<InventorySlot>();
			cSprite = GetComponent<Image>();
            droppedColor = cSprite.color;
		}

		public void OnBeginDrag(PointerEventData eventData) {
			if (cSprite == null) {}

            cSprite.color = cDraggingColor;

            draggedItem = this;
			GhostInventorySlotItem ghostItem = GhostInventorySlotItem.instance;
			ghostItem.cSprite.sprite = cSprite.sprite;
			ghostItem.transform.position = transform.position;
			ghostItem.gameObject.SetActive(true);
		}

		public void OnDrag(PointerEventData eventData) {
            if (draggedItem == null) return;

			GhostInventorySlotItem ghostItem = GhostInventorySlotItem.instance;
			ghostItem.transform.position = eventData.position;
		}

		public void OnEndDrag(PointerEventData eventData) {
            if (draggedItem == null) return;
            cSprite.color = droppedColor;

			GhostInventorySlotItem ghostItem = GhostInventorySlotItem.instance;
			ghostItem.gameObject.SetActive(false);
            draggedItem = null;
		}

		public void OnDrop(PointerEventData eventData) {
            bool shouldSwap = draggedItem != null && draggedItem != this;
            if (!shouldSwap) return;
            GetComponentInParent<Inventory>().Move(draggedItem.cParentSlot, cParentSlot);
        }
	}
}
