using UnityEngine;
using System;

using PrettyPatterns;
using NaughtyAttributes;

namespace eclipse {
	public class StatsModule : Stats.Module<Entity, StatsModule> {}

	public static partial class Alias {
		public static Stats.Module<Entity, StatsModule> stats => StatsModule.instance;
		public static Stats.CoreStats<Entity> coreStats => stats.coreStats;
		public static Stats.CoreStats<Entity> coreStatsBase => stats.coreStatsBase;
		public static Stats.CoreStatsScaling<Entity> coreStatsScaling => stats.coreStatsScaling;
		public static Stats.CoreResource<Entity> coreResource => stats.coreResource;

		public static float[] health => coreResource.health;
		public static float[] maxHealth => coreStats.maxHealth;
		public static float[] defense => coreStats.defense;
		public static float[] damageReduction => coreStats.damageReduction;
		public static float[] movementSpeed => coreStats.movementSpeed;
	}

	public class Entity : MonoBehaviour {
		[ReadOnly]
		public int stats;
		[ReadOnly]
		public int resource;
		[ReadOnly]
		public Optional<int> statScaling;
		[ReadOnly]
		public Optional<int> statBase;
	}

	public static class EntityStatics {
		public static bool IsDead(Entity entity) => Alias.health[entity.resource] <= 0;
		public static void CleanupDead(ReadOnlySpan<Entity> entities) {
			int[] statsToRemove = new int[entities.Length];
			int[] resourcesToRemove = new int[entities.Length];

			int p = 0;
			foreach (Entity entity in entities) {
				statsToRemove[p] = entity.stats;
				resourcesToRemove[p] = entity.resource;
				p++;
			}

			RemovableSpanList.Remove(Alias.coreStats, new ReadOnlySpan<int>(statsToRemove), (int i, int j) => {
                Debug.Log(i + " <- " + j);
                Debug.Log(Alias.coreStats.entities[i] + " " + Alias.coreStats.entities[j]);
				Alias.coreStats.entities[j].stats = i;
				Alias.coreStats.entities[j].resource = i;
			});

			RemovableSpanList.Remove(Alias.coreResource, new ReadOnlySpan<int>(resourcesToRemove), null);

			foreach (Entity entity in entities) UnityEngine.Object.Destroy(entity.gameObject);
		}
	}
}
