using UnityEngine;
using System;

using Stats;
using eclipse.items;

namespace eclipse.trinket {
	[CreateAssetMenu(fileName = "Trinket", menuName = "eclipse/trinket", order = 1)]
	public class Trinket : ItemBlueprint, CanBeApplied {
        public Modifier[] mods;

		public void OnApply(Entity entity) {
			if (mods.Length <= 0) return;
			EnsureModExist(entity);
			foreach (Modifier mod in mods) {
				Alias.coreStatsScaling.Apply(entity.extra.Value.scaling, mod.entry, mod.mod);
			}
		}

		public void OnRemove(Entity entity) {
			if (mods.Length <= 0) return;
			EnsureModExist(entity);
			foreach (Modifier mod in mods) {
				Alias.coreStatsScaling.Apply(entity.extra.Value.scaling, mod.entry, -mod.mod);
			}
		}

		void EnsureModExist(Entity entity) {
            Debug.Log("Ensure mod exist");
			if (entity.extra.IsValid) return;
			EntityStatics.GenerateScaling(new Span<Entity>(new Entity[] { entity }));
		}
    }
}
