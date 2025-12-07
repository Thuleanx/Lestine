using UnityEngine;
using UnityEngine.EventSystems;
using eclipse.items;

namespace eclipse.ui {
	public class InventorySlot : MonoBehaviour, IDropHandler {
		[field:SerializeField]
		public InventorySlotItem cItem { get; private set; }

		[System.NonSerialized]
		public Item item;

		public void SetItem(Item item) {
			this.item = item;
			bool doesSlotHaveItem = item.blueprint != null;
			cItem.cSprite.sprite = doesSlotHaveItem ? item.blueprint.cDescription.cSprite : null;
            cItem.cSprite.enabled = doesSlotHaveItem;
		}

		void Awake() { cItem = GetComponentInChildren<InventorySlotItem>(); }

		public void OnDrop(PointerEventData eventData) {
			bool shouldSwap =
				InventorySlotItem.draggedItem != null && InventorySlotItem.draggedItem.cParentSlot != this;
            if (!shouldSwap) return;
            GetComponentInParent<Inventory>().Move(InventorySlotItem.draggedItem.cParentSlot, this);
		}
	}

}
