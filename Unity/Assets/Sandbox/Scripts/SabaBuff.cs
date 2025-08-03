using System.Runtime.InteropServices;

namespace Saba {
	public enum SabaBuffType : byte {
		Enlarge,
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct SabaBuffData {
		[FieldOffset(0)]
		public readonly SabaBuffType type;
		[FieldOffset(4)]
		public float amount;

		public SabaBuffData(SabaBuffType type) : this() { this.type = type; }

		public static SabaBuffData MakeEnlarge(float amount) {
			SabaBuffData data = new SabaBuffData(SabaBuffType.Enlarge);
			data.amount = amount;
			return data;
		}

		public void ApplyTo(SabaEntity entity) {
			switch (type) {
				case SabaBuffType.Enlarge: {
					entity.transform.localScale *= amount;
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
			}
		}
	}
}
