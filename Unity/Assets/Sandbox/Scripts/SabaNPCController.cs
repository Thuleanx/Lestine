using UnityEngine;
using System.Collections.Generic;

using PrettyPatterns;

namespace Saba {
	public class SabaNPCController : Singleton<SabaNPCController> {
		[SerializeField]
		float separationMaxImpulse = 1;
		[SerializeField]
		float separationRadius = 1;

		List<SabaNPCData> data => SabaNPCRuntimeGroup.instance.data;
		SabaNPCTransientData transientData => SabaNPCRuntimeGroup.instance.transientData;

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

			for (int npcIndex = 0; npcIndex < data.Count; npcIndex++) {
				SabaNPCData npc = data[npcIndex];
				SabaMovementComponent movementComponent = npc.movementComponent;

				float speed = movementComponent.Velocity.magnitude;
				float maxSpeed = SabaAliases.movementSpeed[npc.entity.Attributes];

				float maxImpulse =
					maxSpeed / movementComponent.AccelerationToMaxSpeedSeconds;
				float maxAvoidanceImpulse = maxImpulse;

				Vector2 totalSeparationImpulse = Vector2.zero;

				// separation behavior
				foreach (SabaNPCData otherNPC in data) {
					if (otherNPC.entity == npc.entity) continue;

					Vector2 separation =
						movementComponent.transform.position -
						otherNPC.movementComponent.transform.position;

					float separationDistance = separation.magnitude;

					float combinedRadius = npc.radius + otherNPC.radius;

					float x = (separationDistance - combinedRadius) /
							  separationRadius;

					float separationStrength =
						(x < 1 ? (1 - x) * (1 - x) : 0) * separationMaxImpulse;

					totalSeparationImpulse +=
						separation / separationDistance * separationStrength;
				}

				movementComponent.ApplyForce(
					totalSeparationImpulse * deltaTime
				);

				Vector2 displacementToPlayer =
					player.transform.position - npc.entity.transform.position;
				// Normalize here because distance can be 0
				Vector2 directionToPlayer = displacementToPlayer.normalized;

				// Approach player if hasn't recently hit, otherwise go away
				Vector2 desiredVelocity = maxSpeed * directionToPlayer *
										  (transientData.hasHitPlayerRecently[npcIndex] ? -1 : 1);
				float frameAcceleration = maxImpulse * deltaTime;

				Vector2 desiredAcceleration = Vector2.ClampMagnitude(
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
