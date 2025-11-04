using UnityEngine;
using UnityEngine.Assertions;
using System;

using NaughtyAttributes;
using PrettyPatterns;

namespace Saba {
	// We need this because
	public class SabaStatsModule : Stats.Module<SabaEntity, SabaStatsModule> {}

	public static partial class SabaAliases {
		public static Stats.Module<SabaEntity, SabaStatsModule> allStats => SabaStatsModule.instance;

		public static Stats.CoreStats<SabaEntity> coreStats => allStats.coreStats;
		public static Stats.CoreStats<SabaEntity> coreStatsBase => allStats.coreStatsBase;
		public static Stats.CoreStatsScaling<SabaEntity> coreStatsScaling => allStats.coreStatsScaling;
		public static Stats.CoreResource<SabaEntity> coreResource => allStats.coreResource;

		public static float[] health => coreResource.health;

		public static float[] maxHealth => coreStats.maxHealth;
		public static float[] defense => coreStats.defense;
		public static float[] damageReduction => coreStats.damageReduction;
		public static float[] movementSpeed => coreStats.movementSpeed;
		public static Stats.Modifiers[] damage => coreStats.damage;

		public static Stats.Modifiers[] movementSpeedScaling => coreStatsScaling.movementSpeed;
		public static Stats.Modifiers[] maxHealthScaling => coreStatsScaling.maxHealth;
	}

	public class SabaEntity : MonoBehaviour {
		public int Attributes;
		[ReadOnly]
		public Optional<int> AttributesBase;
		[ReadOnly]
		public Optional<int> AttributesScaling;
		[ReadOnly]
		public int Resource;
		[ReadOnly]
        public Optional<int> StatusEffectContainer;

		public SabaMovementComponent MovementComponent { get; private set; }
		public SabaEffectDispatch EffectDispatch { get; private set; }

		[SerializeField] bool isExecutable = false;

		public bool IsDead {
			get {
				int resourceIndex = Resource;
				return SabaAliases.health[resourceIndex] <= 0;
			}
		}

		void Awake() {
			MovementComponent = GetComponent<SabaMovementComponent>();
			EffectDispatch = GetComponent<SabaEffectDispatch>();
		}

		public void RequestNewAttributesAndBase() {
			Assert.IsTrue(!AttributesBase.IsValid);
			Assert.IsTrue(!AttributesScaling.IsValid);

			// boxing, cry.
			// technically isn't needed if I just repeat the same method in multiple places.
			// C# kinda sucks
			AttributesBase = (SabaAliases.allStats.coreStatsBase as RemovableSpanList).Allocate(1);
			AttributesScaling = (SabaAliases.allStats.coreStatsScaling as RemovableSpanList).Allocate(1);

			SabaAliases.coreStatsBase.Copy(AttributesBase.Value, SabaAliases.coreStats, Attributes);
			SabaAliases.coreStatsScaling.ResetSingle(AttributesScaling.Value);
			SabaAliases.coreStatsScaling.entities[AttributesScaling.Value] = this;
		}

		public void RecomputeStats() {
			bool canRecompute = AttributesBase.IsValid && AttributesScaling.IsValid;
			Assert.IsTrue(canRecompute);
			SabaAliases.allStats.RecomputeStats(Attributes, AttributesBase.Value, AttributesScaling.Value);
		}

		public static void Kill(ReadOnlySpan<SabaEntity> entities) {
			int[] attribute = new int[entities.Length];

			int attributeBaseLength = 0;
			int[] attributeBase = new int[entities.Length];

			int attributeScalingLength = 0;
			int[] attributeScaling = new int[entities.Length];

			int i = 0;
			foreach (SabaEntity entity in entities) {
				attribute[i++] = entity.Attributes;
				if (entity.AttributesBase.IsValid) attributeBase[attributeBaseLength++] = entity.AttributesBase.Value;
				if (entity.AttributesScaling.IsValid)
					attributeScaling[attributeScalingLength++] = entity.AttributesScaling.Value;

                entity.GetComponent<SabaNPC>().enabled = false;
                entity.GetComponent<SabaMovementComponent>()?.Stop();
                // disable collider so we don't get hit again
                entity.GetComponent<Collider2D>().enabled = false;

				bool isExecutable = entity.isExecutable;
				if (isExecutable) {
                    SabaExecutableRuntimeGroup.instance?.Register(entity);
                } else {
                    Destroy(entity.gameObject);
                }
			}

			RemovableSpanList.Remove(SabaAliases.coreStats, new ReadOnlySpan<int>(attribute), (int i, int j) => {
				SabaAliases.coreStats.entities[i].Attributes = i;
				SabaAliases.coreStats.entities[i].Resource = i;
			});
			RemovableSpanList.Remove(SabaAliases.coreResource, new ReadOnlySpan<int>(attribute), null);
			if (attributeBaseLength > 0) {
				RemovableSpanList.Remove(
					SabaAliases.coreStatsBase,
					new ReadOnlySpan<int>(attributeBase, 0, attributeBaseLength),
					(int i, int j) => { SabaAliases.coreStatsBase.entities[i].AttributesBase = i; }
				);
			}
			if (attributeScalingLength > 0) {
				RemovableSpanList.Remove(
					SabaAliases.coreStatsScaling,
					new ReadOnlySpan<int>(attributeBase, 0, attributeScalingLength),
					(int i, int j) => { SabaAliases.coreStatsScaling.entities[i].AttributesScaling = i; }
				);
			}
		}
	}
}
