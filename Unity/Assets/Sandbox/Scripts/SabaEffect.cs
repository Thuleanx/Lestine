using System.Runtime.InteropServices;

namespace Saba {
    [System.Serializable]
	[StructLayout(LayoutKind.Explicit)]
	public struct SabaEffect {
        public enum Type : byte {
            ApplyBuff
        }
		[FieldOffset(0)]
        public Type type;
		[FieldOffset(4)]
        public SabaBuffData buff;
    }
}
