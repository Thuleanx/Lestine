using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

using PrettyPatterns;

namespace Saba {
	public struct SabaNPCData {
		public SabaEntity entity;
		public SabaMovementComponent movementComponent;
		public float radius;

		// methods
		public bool IsValid() => entity && movementComponent;
	}

	public struct SabaNPCTransientData {
		public List<bool> hasHitPlayerRecently;

		public void Expand() { hasHitPlayerRecently.Add(false); }

		public void RemoveAt(int index) {
			hasHitPlayerRecently.RemoveAt(index);
		}
	}

	public class SabaNPCRuntimeGroup : Singleton<SabaNPCRuntimeGroup> {
		public List<SabaNPCData> data = new List<SabaNPCData>();

		public SabaNPCTransientData transientData = new SabaNPCTransientData() {
            hasHitPlayerRecently = new List<bool>()
        };

		public void Register(SabaNPC npc) {
			SabaNPCData data = new SabaNPCData() {
				entity = npc.GetComponent<SabaEntity>(),
				movementComponent = npc.GetComponent<SabaMovementComponent>(),
				radius = npc.Radius,
			};
			Assert.IsTrue(data.IsValid(), "NPC " + npc + " is not valid.");
			this.data.Add(data);
			this.transientData.Expand();
		}

		public void Deregister(SabaNPC npc) {
			int index = data.FindIndex(
				(entry) => entry.entity.gameObject == npc.gameObject
			);
			data.RemoveAt(index);
            transientData.RemoveAt(index);
		}
	}

	[RequireComponent(typeof(SabaEntity))]
	[RequireComponent(typeof(SabaMovementComponent))]
	public class SabaNPC : MonoBehaviour {
		public float Radius;

		public void OnEnable() => SabaNPCRuntimeGroup.instance?.Register(this);
		public void OnDisable() => SabaNPCRuntimeGroup.instance?.Deregister(this
		);
	}
}
