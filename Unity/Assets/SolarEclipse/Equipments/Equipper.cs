using UnityEngine;
using eclipse.items;

namespace eclipse.equipments {
    public enum ItemSlot {
        eHelmet = 0,
        eArmor = 1,
        eGaunlet = 2,
        ePants = 3,
        eNecklace = 4,
        eRing0 = 5,
        eRing1 = 6,
        MAX = eRing1
    }

    [RequireComponent(typeof(Entity))]
	public class Equipper : MonoBehaviour {
        public Item helmet;
        public Item armor;
        public Item gauntlet;
        public Item pants;
        public Item necklace;
        public Item ring_0;
        public Item ring_1;

        public Item this[int key] {
            get => (ItemSlot) key switch
            {
                ItemSlot.eHelmet => helmet,
                ItemSlot.eArmor => armor,
                ItemSlot.eGaunlet => gauntlet,
                ItemSlot.ePants => pants,
            };
        }
	}
}
