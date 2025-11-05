using UnityEngine;

using System.Runtime.CompilerServices;
using PrettyPatterns;

namespace Stats {
    [System.Serializable]
    public struct SingleCoreStatsEntry {
        [Min(0.0f)]
        public float maxHealth;
        [Min(0.0f)]
        public float defense;
        [Min(0.0f)]
        public float damageReduction;
        [Min(0.0f)]
        public float movementSpeed;
        public Modifiers damage;
    }

	[System.Serializable]
	public class CoreStats<T> : RemovableSpanList {
		public int currentNum;
		public float[] maxHealth;
		public float[] defense;
		public float[] damageReduction;
		public float[] movementSpeed;
		public Modifiers[] damage;
		public T[] entities;

		public int GetCapacity() => maxHealth.Length;
		public int GetCurrentNum() => currentNum;
		public void SetCurrentNum(int num) => currentNum = num;

		public static CoreStats<T> Create(int num) {
			CoreStats<T> stats = new CoreStats<T> {
				currentNum = 0,
				maxHealth = new float[num],
				defense = new float[num],
				damageReduction = new float[num],
				movementSpeed = new float[num],
				damage = new Modifiers[num],
				entities = new T[num]
			};
			stats.Reset();
			return stats;
		}

		public void Copy(int tableIndex, CoreStats<T> otherRemovableSpanList, int otherTableIndex) {
			maxHealth[tableIndex] = otherRemovableSpanList.maxHealth[otherTableIndex];
			defense[tableIndex] = otherRemovableSpanList.defense[otherTableIndex];
			damageReduction[tableIndex] = otherRemovableSpanList.damageReduction[otherTableIndex];
			movementSpeed[tableIndex] = otherRemovableSpanList.movementSpeed[otherTableIndex];
			damage[tableIndex] = otherRemovableSpanList.damage[otherTableIndex];
			entities[tableIndex] = otherRemovableSpanList.entities[otherTableIndex];
		}

		public void ResetSingle(int i) {
			maxHealth[i] = defense[i] = damageReduction[i] = movementSpeed[i] = 0.0f;
			damage[i] = Modifiers.Create();
		}

		internal void Reset() {
			for (int i = 0; i < GetCapacity(); i++) {
				maxHealth[i] = defense[i] = damageReduction[i] = movementSpeed[i] = 0.0f;
				damage[i] = Modifiers.Create();
			}
		}

		public void Set(int i, int j) {
			maxHealth[i] = maxHealth[j];
			defense[i] = defense[j];
			damageReduction[i] = damageReduction[j];
			movementSpeed[i] = movementSpeed[j];
			damage[i] = damage[j];
			entities[i] = entities[j];
		}
	}

	[System.Serializable]
	public class CoreStatsScaling<T> : RemovableSpanList {
		public int currentNum;
		public Modifiers[] maxHealth;
		public Modifiers[] defense;
		public Modifiers[] damageReduction;
		public Modifiers[] movementSpeed;
		public Modifiers[] damage;
		public T[] entities;

		public int GetCapacity() => maxHealth.Length;
		public int GetCurrentNum() => currentNum;
		public void SetCurrentNum(int num) => currentNum = num;

		public static CoreStatsScaling<T> Create(int num) {
			CoreStatsScaling<T> stats = new CoreStatsScaling<T> {
				currentNum = 0,
				maxHealth = new Modifiers[num],
				defense = new Modifiers[num],
				damageReduction = new Modifiers[num],
				movementSpeed = new Modifiers[num],
				damage = new Modifiers[num],
				entities = new T[num]
			};
			stats.Reset();
			return stats;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ResetSingle(int i
		) => maxHealth[i] = defense[i] = damageReduction[i] = movementSpeed[i] = damage[i] = Modifiers.Create();

		internal void Reset() {
			for (int i = 0; i < GetCapacity(); i++)
				maxHealth[i] = defense[i] = damageReduction[i] = movementSpeed[i] = damage[i] = Modifiers.Create();
		}

		public void Set(int i, int j) {
			maxHealth[i] = maxHealth[j];
			defense[i] = defense[j];
			damageReduction[i] = damageReduction[j];
			movementSpeed[i] = movementSpeed[j];
			damage[i] = damage[j];
			entities[i] = entities[j];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Apply(int i, ModifierEntry entry, Modifiers modifiers) {
            switch (entry) {
                case ModifierEntry.eMaxHealth:
                    maxHealth[i] += modifiers;
                    break;
                case ModifierEntry.eDefense:
                    defense[i] += modifiers;
                    break;
                case ModifierEntry.eDamageReduction:
                    damageReduction[i] += modifiers;
                    break;
                case ModifierEntry.eMovementSpeed:
                    movementSpeed[i] += modifiers;
                    break;
                case ModifierEntry.eDamage:
                    damage[i] += modifiers;
                    break;
            }
        }
	}

	[System.Serializable]
	public class CoreResource<T> : RemovableSpanList {
		public int currentNum;
		public float[] health;

		public int GetCapacity() => health.Length;
		public int GetCurrentNum() => currentNum;
		public void SetCurrentNum(int num) => currentNum = num;

		public static CoreResource<T> Create(int num) {
			CoreResource<T> resources = new CoreResource<T> { currentNum = 0, health = new float[num] };
			resources.Reset();
			return resources;
		}

		public void ResetSingle(int i) => health[i] = 0.0f;

		internal void Reset() {
			for (int i = 0; i < GetCapacity(); i++) health[i] = 0.0f;
		}

		public void Set(int i, int j) { health[i] = health[j]; }
	}

	public abstract class Module<T, G> : Singleton<G>
		where G : Module<T, G> {
		public CoreStats<T> coreStats;
		public CoreStats<T> coreStatsBase;
		public CoreResource<T> coreResource;
		public CoreStatsScaling<T> coreStatsScaling;

		public override void Awake() {
			base.Awake();

			StatSettings.Value = StatSettings.Get();
			coreStats = CoreStats<T>.Create(StatSettings.Value.NumCoreStats);
			coreResource = CoreResource<T>.Create(StatSettings.Value.NumCoreStats);
			coreStatsBase = CoreStats<T>.Create(StatSettings.Value.NumBaseStats);
			coreStatsScaling = CoreStatsScaling<T>.Create(StatSettings.Value.NumCoreScaling);
		}

		public void RecomputeStats(int coreIndex, int coreBaseIndex, int coreScalingIndex) {
			coreStats.maxHealth[coreIndex] =
				coreStatsScaling.maxHealth[coreScalingIndex].ApplyToBase(coreStatsBase.maxHealth[coreBaseIndex]);
			coreStats.defense[coreIndex] =
				coreStatsScaling.defense[coreScalingIndex].ApplyToBase(coreStatsBase.defense[coreBaseIndex]);
			coreStats.damageReduction[coreIndex] = coreStatsScaling.damageReduction[coreScalingIndex].ApplyToBase(
				coreStatsBase.damageReduction[coreBaseIndex]
			);
			coreStats.movementSpeed[coreIndex] =
				coreStatsScaling.movementSpeed[coreScalingIndex].ApplyToBase(coreStatsBase.movementSpeed[coreBaseIndex]
				);
			coreStats.damage[coreIndex] =
				coreStatsScaling.damage[coreScalingIndex] + coreStatsBase.damage[coreBaseIndex];
		}

		public void InitializeResource(int stat, int resource) {
			coreResource.health[resource] = coreStats.maxHealth[stat];
		}

		// indices should be ordered
	}
}
