using System.Collections.Generic;
using UnityEngine;

using PrettyPatterns;

namespace Saba {
	public class SabaExecutableRuntimeGroup :
		Singleton<SabaExecutableRuntimeGroup> {
		public List<SabaExecutableEntity> activeEntities =
			new List<SabaExecutableEntity>();
		public List<SabaExecutableEntity> inactiveEntities =
			new List<SabaExecutableEntity>();
	}

	[RequireComponent(typeof(SabaEntity))]
	public class SabaExecutableEntity : MonoBehaviour {
		[SerializeField, Range(0, 1)]
		float threshold;

		SabaEntity entity;

		void Awake() { entity = GetComponent<SabaEntity>(); }

		void OnEnable() {
			SabaExecutableRuntimeGroup.instance?.inactiveEntities.Add(this);
		}

		void OnDisable() {
			SabaExecutableRuntimeGroup.instance?.activeEntities.Remove(this
			);
			SabaExecutableRuntimeGroup.instance?.inactiveEntities.Remove(
				this
			);
		}

		public static void UpdateActiveEntities() {
			List<SabaExecutableEntity> newlyActivatedEntities =
				new List<SabaExecutableEntity>();
			foreach (SabaExecutableEntity executableEntity in
						 SabaExecutableRuntimeGroup.instance
							 .inactiveEntities) {
				SabaEntity entity = executableEntity.entity;
				bool shouldActivate =
					entity.Resource.Health <
					entity.Attributes.MaxHealth * executableEntity.threshold;
				if (shouldActivate)
					newlyActivatedEntities.Add(executableEntity);
			}

			SabaExecutableRuntimeGroup.instance.activeEntities.AddRange(
				newlyActivatedEntities
			);
			foreach (SabaExecutableEntity entity in newlyActivatedEntities)
				SabaExecutableRuntimeGroup.instance.inactiveEntities
					.Remove(entity);
		}
	}
}
