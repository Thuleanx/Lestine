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
    };

    public class SabaNPCRuntimeGroup : Singleton<SabaNPCRuntimeGroup> {
        public List<SabaNPCData> data = new List<SabaNPCData>();

		public void Register(SabaNPC npc) {
			SabaNPCData data = new SabaNPCData(
			) { entity = npc.GetComponent<SabaEntity>(),
				movementComponent = npc.GetComponent<SabaMovementComponent>(),
				radius = npc.Radius };
			Assert.IsTrue(data.IsValid(), "NPC " + npc + " is not valid.");
			this.data.Add(data);
		}

		public void Deregister(SabaNPC npc) {
			data.RemoveAll(
				(entry) => entry.entity.gameObject == npc.gameObject
			);
		}
    }

    [RequireComponent(typeof(SabaEntity))]
    [RequireComponent(typeof(SabaMovementComponent))]
	public class SabaNPC : MonoBehaviour {
        public float Radius;

        public void OnEnable() => SabaNPCRuntimeGroup.instance?.Register(this);
        public void OnDisable() => SabaNPCRuntimeGroup.instance?.Deregister(this);
    }
}
