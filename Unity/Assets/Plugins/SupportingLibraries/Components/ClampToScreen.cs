using System.Collections.Generic;
using UnityEngine;

namespace Behaviours {
    public class ClampToScreen : MonoBehaviour {
        Collider Collider;
        Camera mainCamera;
        CharacterController controller;

        [SerializeField] float margin = 0.02f;

        Vector3 lastPosition;

        void Awake() {
            Collider = GetComponent<Collider>();
            controller = GetComponent<CharacterController>();
        }

        void OnEnable() {
            mainCamera = Camera.main;
        }

        void LateUpdate() {
            List<Vector3> positions = new List<Vector3>();

            positions.Add(transform.position);

            if (!Collider) positions.Add(transform.position);
            else {
                for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                for (int dz = -1; dz <= 1; dz++) {
                    positions.Add(Collider.bounds.center
                        + dx * Collider.bounds.extents.x * Vector3.right
                        + dy * Collider.bounds.extents.y * Vector3.up
                        + dz * Collider.bounds.extents.z * Vector3.forward
                    );
                }
            }

            // we correct one position at a time
            foreach (Vector3 position in positions) {
                Vector3 viewportPosition = mainCamera.WorldToViewportPoint(position);
                Vector3 viewportPositionBefore = viewportPosition;

                viewportPosition.x = Mathf.Clamp01(viewportPosition.x);
                viewportPosition.y = Mathf.Clamp01(viewportPosition.y);

                viewportPosition.x = Mathf.Clamp(viewportPosition.x, margin, 1.0f - margin);
                viewportPosition.y = Mathf.Clamp(viewportPosition.y, margin, 1.0f - margin);

                if (Mathf.Approximately((viewportPosition - viewportPositionBefore).sqrMagnitude, 0))
                    continue;

                Ray rayToDesiredSpot = mainCamera.ViewportPointToRay(viewportPosition);

                Vector3 correctedPosition = mainCamera.ViewportToWorldPoint(viewportPosition);

                Vector3 direction = correctedPosition - position;
                direction = Vector3.ProjectOnPlane(direction, rayToDesiredSpot.direction);
                direction.y = 0;

                transform.position = lastPosition;

                direction.Normalize();

                if (controller) controller.Move(direction * 30.0f * Time.deltaTime);
                else            transform.position += direction * 30.0f * Time.deltaTime;

                break;
            }

            lastPosition = transform.position;
        }
    }
}
