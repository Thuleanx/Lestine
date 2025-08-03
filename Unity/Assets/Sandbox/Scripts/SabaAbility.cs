using System.Runtime.InteropServices;
using UnityEngine;

namespace Saba {
	// There are 3 lifetimes for abilities:
	// 1. Single(ton)
	// 2. Per character
	// 3. Per cast

	public interface SabaAbility {
		public enum TargetingType : byte { Location, Entity }

		[StructLayout(LayoutKind.Explicit)]
		public struct Target {
			[FieldOffset(0)]
			public Vector2 location;
			[FieldOffset(0)]
			public SabaEntity entity;

			[FieldOffset(8)]
			public TargetingType type;

			public static implicit operator Target(Vector2 location
			) => new Target() {
				type = TargetingType.Location, location = location
			};

			public static implicit operator Target(SabaEntity entity
			) => new Target() { type = TargetingType.Entity, entity = entity };
		}

		public void Activate(SabaEntity entity, SabaAbility.Target target);
	}

	public interface SabaAbilityInstance {
		public void Activate(SabaAbility.Target target);
		public bool IsTickable() => false;
		public void Tick() {}
		public void Deactivate() {}
	}
}
