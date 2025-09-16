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
			switch (type) {
				case SabaBuffType.Enlarge: {
					entity.transform.localScale *= amount;
					break;
				}
				case SabaBuffType.SpeedIncrease: {
					entity.AttributesScaling.MovementSpeed.Increase += amount;
					break;
				}
                case SabaBuffType.MaxHealthFlat: {
                    entity.AttributesScaling.MaxHealth.Added += amount;
                    break;
                }
			}
            entity.ComputeAttributes();
		}

		public void RemoveFrom(SabaEntity entity) {
			switch (type) {
				case SabaBuffType.Enlarge: {
					entity.transform.localScale /= amount;
					break;
				}
				case SabaBuffType.SpeedIncrease: {
					entity.AttributesScaling.MovementSpeed.Increase -= amount;
					break;
				}
                case SabaBuffType.MaxHealthFlat: {
                    entity.AttributesScaling.MaxHealth.Added -= amount;
                    break;
                }
			}
            entity.ComputeAttributes();
		}
	}
}
