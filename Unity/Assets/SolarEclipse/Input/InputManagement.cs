using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

using ADammy;

namespace eclipse.input {
	public static class Movement {
		public static Vector2 Value;
	}

	public static class PointerPosition {
		public static Vector2 Value;
	}

	public class InputManagement : MonoBehaviour {
		public void OnMovement(InputAction.CallbackContext ctx) {
			Assert.IsTrue(ctx.valueType.Equals(typeof(Vector2)));
			Movement.Value = ctx.ReadValue<Vector2>();
		}

		public void OnPointerPosition(InputAction.CallbackContext ctx) {
			Assert.IsTrue(ctx.valueType.Equals(typeof(Vector2)));
			PointerPosition.Value = ctx.ReadValue<Vector2>();
		}

		public void OnAttack(InputAction.CallbackContext ctx) {
			if (ctx.started) EventBus<AttackAction>.Raise(new AttackAction() { active = true });
			if (ctx.canceled) EventBus<AttackAction>.Raise(new AttackAction() { active = false });
		}

		public void OnExecute(InputAction.CallbackContext ctx) {
			if (ctx.started) EventBus<ExecutionAction>.Raise();
		}

		public void OnInteract(InputAction.CallbackContext ctx) {
			if (ctx.started) EventBus<InteractionAction>.Raise();
		}
	}
}
