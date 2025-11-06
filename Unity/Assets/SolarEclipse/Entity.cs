using UnityEngine;
using UnityEngine.Assertions;
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
		[System.Serializable]
		public struct ExtraIndices {
			public int scaling;
			public int @base;
		}

		[ReadOnly]
		public int stats;
		[ReadOnly]
		public int resource;
		[ReadOnly]
		public Optional<ExtraIndices> extra;
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

			RemovableSpanList.Remove(Alias.coreResource, resourcesToRemove, null);
			RemovableSpanList.Remove(Alias.coreStats, statsToRemove, (int i, int j) => {
				Alias.coreStats.entities[j].stats = i;
				Alias.coreStats.entities[j].resource = i;
			});

			foreach (Entity entity in entities) UnityEngine.Object.Destroy(entity.gameObject);
		}

		public static void GenerateScaling(Span<Entity> entities) {
			int baseStats = (Alias.coreStatsBase as RemovableSpanList).Allocate(entities.Length);
			int scalingStats = (Alias.coreStats as RemovableSpanList).Allocate(entities.Length);

			int p = 0;
			foreach (Entity entity in entities) {
				Assert.IsTrue(!entity.extra.IsValid, "Requesting generate scaling on entity that already has it");
				entity.extra = new Entity.ExtraIndices { scaling = scalingStats + p, @base = baseStats + p };

				Alias.coreStatsBase.Copy(entity.extra.Value.@base, Alias.coreStats, entity.stats);
				Alias.coreStatsScaling.ResetSingle(entity.extra.Value.scaling);

				p++;
			}
		}

		public static void RecomputeStats(Entity entity) {
			Assert.IsTrue(entity.extra.IsValid, "Recomputing stats for entity, but no extras field.");
			Alias.stats.RecomputeStats(entity.stats, entity.extra.Value.@base, entity.extra.Value.scaling);
		}
	}
}
