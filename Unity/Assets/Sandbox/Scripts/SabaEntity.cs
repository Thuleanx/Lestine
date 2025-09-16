using UnityEngine;
using System.Collections.Generic;

using NaughtyAttributes;

namespace Saba {
	public class SabaEntity : MonoBehaviour {
		[ReadOnly]
		public SabaAttributes Attributes =
			new SabaAttributes() { DamageScaling = new SabaAttributeCoefficients() { More = 1 } };
		[ReadOnly]
		public SabaResource Resource;
		public SabaAttributes AttributesBase =
			new SabaAttributes() { DamageScaling = new SabaAttributeCoefficients() { More = 1 } };
		public SabaMovementComponent MovementComponent { get; private set; }

		// There's currently no default struct initialization in c#9 so we'll have to do it this way
		public SabaAttributeScaling AttributesScaling = new SabaAttributeScaling() {
			MaxHealth = new SabaAttributeCoefficients { More = 1.0f },
			Defense = new SabaAttributeCoefficients { More = 1.0f },
			DamageReduction = new SabaAttributeCoefficients { More = 1.0f },
			MovementSpeed = new SabaAttributeCoefficients { More = 1.0f },
			Damage = new SabaAttributeCoefficients { More = 1.0f },
		};

		[SerializeField]
		bool isExecutable = false;

		public bool IsDead => Resource.Health <= 0;

		void Awake() { MovementComponent = GetComponent<SabaMovementComponent>(); }

		void OnEnable() {
			InitializeResources();
			ComputeAttributes();
		}

		void InitializeResources() { Resource.Health = Attributes.MaxHealth; }

		public void ComputeAttributes() {
			Attributes.MaxHealth = AttributesScaling.MaxHealth.Apply(AttributesBase.MaxHealth);
			Attributes.Defense = AttributesScaling.Defense.Apply(AttributesBase.Defense);
			Attributes.DamageReduction = AttributesScaling.DamageReduction.Apply(AttributesBase.DamageReduction);
			Attributes.MovementSpeed = AttributesScaling.MovementSpeed.Apply(AttributesBase.MovementSpeed);
			Attributes.DamageScaling = AttributesScaling.Damage + AttributesBase.DamageScaling;
		}

		public static void Kill(IEnumerable<SabaEntity> entities) {
			foreach (SabaEntity entity in entities) {
				bool isExecutable = entity.isExecutable;

				if (!isExecutable) {
					Destroy(entity.gameObject);
					return;
				}

				entity.GetComponent<SabaNPC>().enabled = false;
				entity.GetComponent<SabaMovementComponent>()?.Stop();
				SabaExecutableRuntimeGroup.instance?.Register(entity);
			}
		}
	}
}
