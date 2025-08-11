using System.Runtime.InteropServices;

namespace Saba {
	public enum SabaBuffType : byte {
		Enlarge,
        Speedup,
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct SabaBuffData {
		[FieldOffset(0)]
		public readonly SabaBuffType type;
        [FieldOffset(1)]
        public bool stackable;
		[FieldOffset(4)]
		public float amount;

		public SabaBuffData(SabaBuffType type) : this() { this.type = type; }

		public static SabaBuffData MakeEnlarge(float amount, bool stackable = false) {
			SabaBuffData data = new SabaBuffData(SabaBuffType.Enlarge);
            data.stackable = stackable;
			data.amount = amount;
			return data;
		}

        public static SabaBuffData MakeSpeedup(float amount, bool stackable = false) {
			SabaBuffData data = new SabaBuffData(SabaBuffType.Speedup);
            data.stackable = stackable;
			data.amount = amount;
			return data;
        }

		public void ApplyTo(SabaEntity entity) {
			switch (type) {
				case SabaBuffType.Enlarge: {
					entity.transform.localScale *= amount;
					break;
				}
                case SabaBuffType.Speedup: {
                    entity.Attributes.MovementSpeed += amount;
                    break;
                }
			}
		}

		public void RemoveFrom(SabaEntity entity) {
			switch (type) {
				case SabaBuffType.Enlarge: {
					entity.transform.localScale /= amount;
					break;
				}
                case SabaBuffType.Speedup: {
                    entity.Attributes.MovementSpeed -= amount;
                    break;
                }
			}
		}
	}
}
