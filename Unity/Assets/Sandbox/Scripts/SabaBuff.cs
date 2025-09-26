using UnityEngine;
using System.Runtime.InteropServices;

namespace Saba {
    [System.Serializable]
	public enum SabaBuffType : byte {
		Enlarge,
		SpeedIncrease,
        MaxHealthFlat,
	}

    [System.Serializable]
	[StructLayout(LayoutKind.Explicit)]
	public struct SabaBuffData {
		[FieldOffset(0)]
		public SabaBuffType type;
		[FieldOffset(1)]
		public bool stackable;
		[FieldOffset(4)]
		public float amount;

		public SabaBuffData(SabaBuffType type) : this() { this.type = type; }

        public static SabaBuffData Make(SabaBuffType type, float amount, bool stackable = false) {
            return new SabaBuffData() {
                type = type,
                stackable = stackable,
                amount = amount
            };
        }

		public void ApplyTo(SabaEntity entity) {
            bool needCreateNewScalingEntry = !entity.AttributesScaling.IsValid;
            if (needCreateNewScalingEntry) entity.RequestNewAttributesAndBase();

			switch (type) {
				case SabaBuffType.Enlarge: {
					entity.transform.localScale *= amount;
					break;
				}
				case SabaBuffType.SpeedIncrease: {
					SabaAliases.movementSpeedScaling[entity.AttributesScaling.Value].increase += amount;
					break;
				}
                case SabaBuffType.MaxHealthFlat: {
					SabaAliases.maxHealthScaling[entity.AttributesScaling.Value].added += amount;
                    break;
                }
			}
            entity.RecomputeStats();
		}

		public void RemoveFrom(SabaEntity entity) {
            bool needCreateNewScalingEntry = !entity.AttributesScaling.IsValid;
            if (needCreateNewScalingEntry) entity.RequestNewAttributesAndBase();

			switch (type) {
				case SabaBuffType.Enlarge: {
					entity.transform.localScale /= amount;
					break;
				}
				case SabaBuffType.SpeedIncrease: {
					SabaAliases.movementSpeedScaling[entity.AttributesScaling.Value].increase -= amount;
					break;
				}
                case SabaBuffType.MaxHealthFlat: {
					SabaAliases.maxHealthScaling[entity.AttributesScaling.Value].added -= amount;
                    break;
                }
			}
            entity.RecomputeStats();
		}
	}
}
