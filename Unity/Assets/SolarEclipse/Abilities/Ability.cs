using UnityEngine;

namespace eclipse.abilities {
	public abstract class Ability {
        public Sprite icon;
        public float cooldown;
        public float healthCost;

        public bool CanAffordAbility(Entity entity) {
            return true;
        }

        public abstract AbilityInstance MakeInstance();
	}

    public abstract class AbilityInstance {
        public Ability ability;

        public abstract void Activate();
        public abstract void Deactivate();
    }
}
