using UnityEngine;

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
}
