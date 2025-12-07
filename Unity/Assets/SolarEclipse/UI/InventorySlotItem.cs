using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using DG.Tweening;
using NaughtyAttributes;

namespace eclipse.ui {
	[RequireComponent(typeof(Image))]
	public class InventorySlotItem :
		MonoBehaviour,
		IBeginDragHandler,
		IDragHandler,
		IEndDragHandler,
		IDropHandler,
		IPointerEnterHandler,
		IPointerExitHandler {
		[field:SerializeField]
		public Color cDraggingColor { get; private set; }
		public Color cDroppedColor { get; private set; }
		[field:SerializeField, MinMaxSlider(0, 3)]
		public Vector2 cTooltipAppearDelaySeconds {
			get; private set;
		}

		public InventorySlot cParentSlot { get; private set; }
		public Image cSprite { get; private set; }

		public static InventorySlotItem draggedItem { get; private set; }

		Tween displayDelayedCall;

		void Awake() {
			cParentSlot = GetComponentInParent<InventorySlot>();
			cSprite = GetComponent<Image>();
			cDroppedColor = cSprite.color;
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
			cSprite.color = cDroppedColor;

			GhostInventorySlotItem ghostItem = GhostInventorySlotItem.instance;
			ghostItem.gameObject.SetActive(false);
			draggedItem = null;
		}

		public void OnDrop(PointerEventData _) {
			bool shouldSwap = draggedItem != null && draggedItem != this;
			if (!shouldSwap) return;
			GetComponentInParent<Inventory>().Move(draggedItem.cParentSlot, cParentSlot);
			displayDelayedCall = DOVirtual.DelayedCall(
				cTooltipAppearDelaySeconds.y - cTooltipAppearDelaySeconds.x, () => OnPointerEnter(default)
			);
		}

		public void OnPointerEnter(PointerEventData _) {
			displayDelayedCall = DOVirtual.DelayedCall(cTooltipAppearDelaySeconds.x, () => {
				Tooltip tooltip = Tooltip.instance;
				bool isTooltipShowing = tooltip.Owner != null;
				bool isDragging = draggedItem != null;
				if (isTooltipShowing || isDragging) return;

				eclipse.items.ItemBlueprint blueprint = cParentSlot.item.blueprint;

				Tooltip.Content content = new Tooltip.Content(
				) { title = blueprint.cDescription.cDisplayName,
					description = blueprint.cDescription.cDescriptionText,
					image = blueprint.cDescription.cSprite };
				tooltip.Show(content, this);
			});
		}

		public void OnPointerExit(PointerEventData _) {
			displayDelayedCall?.Kill();
			Tooltip tooltip = Tooltip.instance;
			if (tooltip.Owner != this) return;
			tooltip.Hide();
		}
	}
}
