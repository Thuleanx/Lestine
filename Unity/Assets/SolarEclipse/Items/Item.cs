using UnityEngine;
using System;

using Stats;

namespace eclipse.items {
	public class Item : ScriptableObject {
        [System.Serializable]
        public struct Modifier {
            public ModifierEntry entry;
            public Modifiers mod;
        };

        public Modifier[] mods;

		public void OnAdd(Entity entity) {
			if (mods.Length <= 0) return;
			EnsureModExist(entity);
			foreach (Item.Modifier mod in mods) {
				Alias.coreStatsScaling.Apply(entity.extra.Value.scaling, mod.entry, mod.mod);
			}
		}

		public void OnRemove(Entity entity) {
			if (mods.Length <= 0) return;
			EnsureModExist(entity);
			foreach (Item.Modifier mod in mods) {
				Alias.coreStatsScaling.Apply(entity.extra.Value.scaling, mod.entry, -mod.mod);
			}
		}

		void EnsureModExist(Entity entity) {
			if (entity.extra.IsValid) return;
			EntityStatics.GenerateScaling(new Span<Entity>(new Entity[] { entity }));
		}
	}
}
