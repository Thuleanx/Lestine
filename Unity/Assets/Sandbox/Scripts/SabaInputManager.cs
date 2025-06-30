using UnityEngine;
using UnityEngine.InputSystem;
using Scriptables;
using UnityEngine.Assertions;

namespace Saba {
	public class SabaInputManager : MonoBehaviour {
        [SerializeField] ScriptableVector2 MovementInput;

        public void OnMovement(InputAction.CallbackContext context) {
            Assert.IsTrue(context.valueType.Equals(typeof(Vector2)));
            MovementInput.Value = context.ReadValue<Vector2>();
        }
	}
}
