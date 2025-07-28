using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

using PrettyPatterns;

namespace Saba {
	public class SabaNPCController : Singleton<SabaNPCController> {
		struct NPCData {
			public SabaEntity entity;
			public SabaMovementComponent movementComponent;
			public float radius;

			// methods
			public bool IsValid() => entity && movementComponent;
		};

		List<NPCData> data = new List<NPCData>();

        [SerializeField] float separationMaxImpulse = 1;
        [SerializeField] float separationRadius = 1; 

		public void Register(SabaNPC npc) {
			NPCData data = new NPCData(
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

            float deltaTime = Time.deltaTime;

			foreach (NPCData npc in data) {
				SabaMovementComponent movementComponent = npc.movementComponent;

				float speed = movementComponent.Velocity.magnitude;
				float maxSpeed = npc.entity.Attributes.MovementSpeed;

                float maxImpulse = maxSpeed / movementComponent.AccelerationToMaxSpeedSeconds;
				float maxAvoidanceImpulse = maxImpulse;

				Vector3 totalSeparationImpulse = Vector3.zero;

				foreach (NPCData otherNPC in data) {
					if (otherNPC.entity == npc.entity) continue;

					// separation behavior
					Vector3 separation =
						movementComponent.transform.position -
						otherNPC.movementComponent.transform.position;

                    float separationDistance = separation.magnitude;

                    float combinedRadius = npc.radius + otherNPC.radius;

                    float x = (separationDistance - combinedRadius) / separationRadius;

                    float separationStrength = (x < 1 ? (1-x) * (1-x) : 0) * separationMaxImpulse;

                    totalSeparationImpulse += separation / separationDistance * separationStrength;
				}

                movementComponent.ApplyForce(totalSeparationImpulse * deltaTime);

				Vector3 directionToPlayer =
					player.transform.position - npc.entity.transform.position;
				directionToPlayer.z = 0;
				directionToPlayer.Normalize();


				Vector3 desiredVelocity = maxSpeed * directionToPlayer;
				float frameAcceleration = maxImpulse * deltaTime;

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
