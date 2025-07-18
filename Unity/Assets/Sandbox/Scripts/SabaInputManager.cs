using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;

using Scriptables;
using ADammy;

namespace Saba {
	public class SabaInputManager : MonoBehaviour {
        [SerializeField] ScriptableVector2 MovementInput;
        [SerializeField] ScriptableVector2 MouseInput;

        public void OnMovement(InputAction.CallbackContext context) {
            Assert.IsTrue(context.valueType.Equals(typeof(Vector2)));
            MovementInput.Value = context.ReadValue<Vector2>();
        }

        public void OnAttack(InputAction.CallbackContext context) {
            if (context.started) EventBus<AttackAction>.Raise(new AttackAction() { active = true});
            if (context.canceled) EventBus<AttackAction>.Raise(new AttackAction() { active = false });
        }

        public void OnPointerPosition(InputAction.CallbackContext context) {
            Assert.IsTrue(context.valueType.Equals(typeof(Vector2)));
            MouseInput.Value = context.ReadValue<Vector2>();
        }
	}
}
