using UnityEngine;
using eclipse.items;

namespace eclipse.inventory {
	public class Inventory : MonoBehaviour {
        public Item[] items;
        public int Size => items.Length;
	}
}
