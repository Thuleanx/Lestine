using UnityEngine;

using eclipse.player;
using MathUtils;

namespace eclipse.camera {
	[ExecuteAlways]
	[RequireComponent(typeof(Camera))]
	public class CameraController : MonoBehaviour {
		Camera cameraCached;

		const float maxViewportDistance = 0.5f;

		[SerializeField]
		float distanceToPlayerPlane = 10.0f;

		[SerializeField, Range(0, 0.99f)]
		float deadZone;

		[SerializeField, Range(0, 0.99f)]
		float maxRatio;

        [SerializeField]
        float smoothing;

		void Awake() { cameraCached = GetComponent<Camera>(); }

		void LateUpdate() { UpdateCameraPosition(); }

		void UpdateCameraPosition() {
			Vector2 viewportMiddle = Vector2.one / 2.0f;

			Vector2 playerPosition;
			if (PlayerTransform.Value) playerPosition = PlayerTransform.Value.position;
			else {
				Player player = FindObjectOfType<Player>();
				if (!player) return;
				playerPosition = player.transform.position;
			}

			Vector2 screenSpacePosition = eclipse.input.PointerPosition.Value;
			Vector2 viewportSpacePosition =
				Application.isPlaying ? cameraCached.ScreenToViewportPoint(screenSpacePosition) : viewportMiddle;

			if (Mathf.Abs(viewportSpacePosition.x - 0.5f) > 0.5f)
				viewportSpacePosition =
					(viewportSpacePosition - viewportMiddle) / (2 * Mathf.Abs(viewportSpacePosition.x - 0.5f)) +
					viewportMiddle;
			if (Mathf.Abs(viewportSpacePosition.y - 0.5f) > 0.5f)
				viewportSpacePosition =
					(viewportSpacePosition - viewportMiddle) / (2 * Mathf.Abs(viewportSpacePosition.y - 0.5f)) +
					viewportMiddle;

			float normalizedViewportDistanceFromCenter =
				Mathf.Clamp01(((viewportSpacePosition - viewportMiddle).magnitude) / maxViewportDistance);
			float distanceFromCenterWithDeadzone =
				Mathf.Max(0, (normalizedViewportDistanceFromCenter - deadZone) / (1 - deadZone));

            float t = distanceFromCenterWithDeadzone;
            float playerToCenterRatio = Mathf.Lerp(0, maxRatio, t * (2 - t));

			Vector2 playerViewportPosition =
				-(viewportSpacePosition - viewportMiddle) * playerToCenterRatio / (1 - playerToCenterRatio) +
				viewportMiddle;

			Ray toPlayerRay = cameraCached.ViewportPointToRay(playerViewportPosition);

			Ray playerToCameraRay = new Ray(playerPosition, -toPlayerRay.direction);

            Vector3 desiredPosition = playerToCameraRay.GetPoint(distanceToPlayerPlane);
            Vector3 nextPosition = Mathx.Damp(Vector3.Lerp, transform.position, desiredPosition, smoothing, Time.deltaTime);
			transform.position = nextPosition;
		}
	}
}
