using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

using PrettyPatterns;

namespace Saba {
	public class SabaNPCController : Singleton<SabaNPCController> {
		struct NPCData {
			public SabaEntity entity;
			public SabaMovementComponent movementComponent;

			// methods
			public bool IsValid() => entity && movementComponent;
		};

		List<NPCData> data = new List<NPCData>();

		public void Register(SabaNPC npc) {
			NPCData data = new NPCData(
			) { entity = npc.GetComponent<SabaEntity>(),
				movementComponent = npc.GetComponent<SabaMovementComponent>() };
			Assert.IsTrue(data.IsValid(), "NPC " + npc + " is not valid.");
			this.data.Add(data);
		}

		public void Deregister(SabaNPC npc) {
			data.RemoveAll(
				(entry) => entry.entity.gameObject == npc.gameObject
			);
		}

		public void OnEntitiesKilled(IEnumerable<SabaEntity> entities) {
			foreach (SabaEntity entity in entities) {
				int index = data.FindIndex(
					0, data.Count, (entry) => entry.entity == entity
				);
				data.RemoveAt(index);
			}
		}

		void Update() {
			SabaPlayer player = SabaPlayer.instance;
			if (!player) return;

			foreach (NPCData npc in data) {
				SabaMovementComponent movementComponent = npc.movementComponent;

				Vector3 directionToPlayer =
					player.transform.position - npc.entity.transform.position;
				directionToPlayer.y = 0;
				directionToPlayer.Normalize();

				float maxSpeed = npc.entity.Attributes.MovementSpeed;

				Vector3 desiredVelocity = maxSpeed * directionToPlayer;
				float frameAcceleration =
					maxSpeed / movementComponent.AccelerationToMaxSpeedSeconds *
					Time.deltaTime;

				Vector3 desiredAcceleration = Vector3.ClampMagnitude(
					desiredVelocity - movementComponent.Velocity,
					frameAcceleration
				);

				movementComponent.ApplyForce(
					desiredAcceleration * movementComponent.Mass
				);
			}
		}
	}
}
