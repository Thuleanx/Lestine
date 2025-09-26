using System;
using System.Collections.Generic;
using UnityEngine;

using PrettyPatterns;

namespace Stats {
	[System.Serializable]
	public struct Scaling {
		public float added;
		public float increase;
		public float more;

		public static Scaling Create() { return new Scaling { more = 1.0f }; }
		public float ApplyToBase(float @base) => (@base + added) * (1.0f + increase) * more;

		public static Scaling operator +(Scaling a, Scaling b) => new Scaling() {
			added = a.added + b.added, increase = a.increase + b.increase, more = a.more * b.more
		};
	}

	public interface Table {
		public int GetCapacity();
		public int GetCurrentNum();
		public void SetCurrentNum(int num);
		public void Set(int i, int j);
		public void ResetSingle(int i);

		public int Allocate(int number) {
			int current = GetCurrentNum();
			SetCurrentNum(current + number);
			return current;
		}

		public static void Remove(Table table, ReadOnlySpan<int> indices, Action<int, int> onMove) {
			// we need to update stat table references of certain entities when
			// we kill some and remap the indices
			Dictionary<int, int> remapping = new Dictionary<int, int>();


            foreach (int index in indices) {
				int lastIndex = table.GetCurrentNum() - 1;

				int indexToRemove = index;

				bool previouslyMoved = remapping.ContainsKey(index);
				if (previouslyMoved) {
					indexToRemove = remapping[index];
					remapping.Remove(index);
				}

				if (lastIndex != indexToRemove) {
					remapping[lastIndex] = indexToRemove;
					table.Set(indexToRemove, lastIndex);
                    if (onMove != null) onMove(indexToRemove, lastIndex);
				}

				table.SetCurrentNum(lastIndex);
			}
		}
	}

	[System.Serializable]
	public class CoreStats<T> : Table {
		public int currentNum;
		public float[] maxHealth;
		public float[] defense;
		public float[] damageReduction;
		public float[] movementSpeed;
		public Scaling[] damage;
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
				damage = new Scaling[num],
				entities = new T[num]
			};
			stats.Reset();
			return stats;
		}

		public void Copy(int tableIndex, CoreStats<T> otherTable, int otherTableIndex) {
			maxHealth[tableIndex] = otherTable.maxHealth[otherTableIndex];
			defense[tableIndex] = otherTable.defense[otherTableIndex];
			damageReduction[tableIndex] = otherTable.damageReduction[otherTableIndex];
			movementSpeed[tableIndex] = otherTable.movementSpeed[otherTableIndex];
			damage[tableIndex] = otherTable.damage[otherTableIndex];
			entities[tableIndex] = otherTable.entities[otherTableIndex];
		}

		public void ResetSingle(int i) {
			maxHealth[i] = defense[i] = damageReduction[i] = movementSpeed[i] = 0.0f;
			damage[i] = Scaling.Create();
		}

		internal void Reset() {
			for (int i = 0; i < GetCapacity(); i++) {
				maxHealth[i] = defense[i] = damageReduction[i] = movementSpeed[i] = 0.0f;
				damage[i] = Scaling.Create();
			}
		}

		public void Set(int i, int j) {
			maxHealth[i] = maxHealth[j];
			defense[i] = defense[j];
			damageReduction[i] = damageReduction[j];
			movementSpeed[i] = movementSpeed[j];
			damage[i] = damage[j];
			entities[i] = entities[j];
			entities[j] = default;	// invalidate reference
		}
	}

	[System.Serializable]
	public class CoreStatsScaling<T> : Table {
		public int currentNum;
		public Scaling[] maxHealth;
		public Scaling[] defense;
		public Scaling[] damageReduction;
		public Scaling[] movementSpeed;
		public Scaling[] damage;
		public T[] entities;

		public int GetCapacity() => maxHealth.Length;
		public int GetCurrentNum() => currentNum;
		public void SetCurrentNum(int num) => currentNum = num;

		public static CoreStatsScaling<T> Create(int num) {
			CoreStatsScaling<T> stats = new CoreStatsScaling<T> {
				currentNum = 0,
				maxHealth = new Scaling[num],
				defense = new Scaling[num],
				damageReduction = new Scaling[num],
				movementSpeed = new Scaling[num],
				damage = new Scaling[num],
				entities = new T[num]
			};
			stats.Reset();
			return stats;
		}

		public void ResetSingle(int i
		) => maxHealth[i] = defense[i] = damageReduction[i] = movementSpeed[i] = damage[i] = Scaling.Create();
		internal void Reset() {
			for (int i = 0; i < GetCapacity(); i++)
				maxHealth[i] = defense[i] = damageReduction[i] = movementSpeed[i] = damage[i] = Scaling.Create();
		}

		public void Set(int i, int j) {
			maxHealth[i] = maxHealth[j];
			defense[i] = defense[j];
			damageReduction[i] = damageReduction[j];
			movementSpeed[i] = movementSpeed[j];
			damage[i] = damage[j];
			entities[i] = entities[j];
			entities[j] = default;
		}
	}

	[System.Serializable]
	public class CoreResource<T> : Table {
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
