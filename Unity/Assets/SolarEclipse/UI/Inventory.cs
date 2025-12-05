using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections.Generic;

using eclipse.items;

namespace eclipse.ui {
	[RequireComponent(typeof(ScrollRect))]
	public class Inventory : MonoBehaviour {
		[field:SerializeField]
		public InventorySlot cSlotPrefab { get; private set; }
		public ScrollRect cScrollRect { get; private set; }

		[field:SerializeField]
		public eclipse.inventory.Inventory TrackedInventory {
			get; private set;
		}

		List<InventorySlot> spawnedSlots = new List<InventorySlot>();

		void Awake() { cScrollRect = GetComponent<ScrollRect>(); }

        void Start() {
            if (TrackedInventory != null) Display(TrackedInventory);
        }

		public void Display(eclipse.inventory.Inventory inventory) {
			Assert.IsNotNull(inventory);
			TrackedInventory = inventory;

			while (spawnedSlots.Count < inventory.Size) {
				InventorySlot newSlot = Instantiate(cSlotPrefab, cScrollRect.content.transform);
				spawnedSlots.Add(newSlot);
			}

			for (int i = 0; i < spawnedSlots.Count; i++) spawnedSlots[i].gameObject.SetActive(i < inventory.Size);
			for (int i = 0; i < inventory.Size; i++)
                spawnedSlots[i].SetItem(inventory.items[i]);
		}

        public void Move(InventorySlot a, InventorySlot b) {
            int aIndex = spawnedSlots.IndexOf(a);
            int bIndex = spawnedSlots.IndexOf(b);

            if (aIndex == bIndex) return;

            Item aItem = a.item;
            Item bItem = b.item;

            TrackedInventory.items[aIndex] = bItem;
            TrackedInventory.items[bIndex] = aItem;
            a.SetItem(bItem);
            b.SetItem(aItem);
        }
	}
}
