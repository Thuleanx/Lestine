using UnityEngine;

using MathUtils;

namespace Saba {
	[ExecuteAlways]
	[RequireComponent(typeof(Camera))]
	public class SabaCameraController : MonoBehaviour {
		new Camera camera;

		[SerializeField]
		float approachAlpha;
		[SerializeField]
		float distanceToTarget;

		void Awake() { camera = GetComponent<Camera>(); }

		void Start() { Refocus(); }

		public Vector3 GetDesiredFocusPosition() {
			SabaPlayer player = SabaPlayer.instance;

			Vector3 desiredFocusPosition = player.transform.position;
			desiredFocusPosition.z = 0;

			return desiredFocusPosition;
		}

		public Vector3 GetArmPosition(Vector3 focusPosition
		) => focusPosition + camera.transform.forward * -distanceToTarget;

		public void Refocus() {
			transform.position = GetArmPosition(GetDesiredFocusPosition());
		}

		void Update() {
			float deltaTime = Time.deltaTime;

			Vector3 desiredArmPosition = GetArmPosition(GetDesiredFocusPosition());
			Vector3 nextArmPosition = Mathx.Damp(
				Vector3.Lerp,
				transform.position,
				desiredArmPosition,
				approachAlpha,
				deltaTime
			);

            transform.position = nextArmPosition;
		}
	}
}
