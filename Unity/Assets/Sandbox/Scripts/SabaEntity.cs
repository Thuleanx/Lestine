using UnityEngine;
using System.Collections.Generic;

using NaughtyAttributes;

namespace Saba {
	public class SabaEntity : MonoBehaviour {
		public SabaAttributes Attributes;
		[ReadOnly]
		public SabaResource Resource;

		[SerializeField]
		bool isExecutable = false;

		public bool IsDead => Resource.Health <= 0;

		void Awake() { Resource.Health = Attributes.MaxHealth; }

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
