using UnityEngine;

namespace Saba {
	[RequireComponent(typeof(SabaEntity))]
	public class SabaBuffContainer : MonoBehaviour {
		const int MAX_BUFFS = 100;

		struct Data {
			public float expireTime;
			public SabaBuffData buffData;
		};

		SabaEntity entity;
		Data[] data = new Data[MAX_BUFFS];
		int buffCount = 0;

		void Awake() { entity = GetComponent<SabaEntity>(); }

		public void ApplyBuff(SabaBuffData buffData, float durationSeconds) {
			if (buffCount == MAX_BUFFS) {
				Debug.Log(
					"Cannot add additional buffs, currently has " + MAX_BUFFS +
					" active"
				);
				return;
			}

			enabled = true;

			if (!buffData.stackable) {
				int index = -1;

				// Since the number of buffs is low linear search should be okay
				for (int i = 0; i < buffCount; i++)
					if (data[i].buffData.type == buffData.type) index = i;

				if (index != -1) {
					// found another instance
					bool shouldOverride =
						data[index].buffData.amount < buffData.amount;
					if (shouldOverride) {
						data[index].buffData.RemoveFrom(entity);
						buffData.ApplyTo(entity);

						data[index] = new Data {
							expireTime = Time.time + durationSeconds,
							buffData = buffData
						};

					} else return;
				}
			}

			buffData.ApplyTo(entity);
			data[buffCount++] = new Data {
				expireTime = Time.time + durationSeconds, buffData = buffData
			};
		}

		void Update() {
			// remove expired buffs
			for (int i = 0; i < buffCount; i++) {
				bool isExpired = data[i].expireTime < Time.time;
				if (isExpired) {
					data[i].buffData.RemoveFrom(entity);
					data[i] = data[buffCount - 1];
					buffCount--;
				}
			}

			// no need to update, if there's no buff on this entity
			if (buffCount == 0) enabled = false;
		}
	}
}
