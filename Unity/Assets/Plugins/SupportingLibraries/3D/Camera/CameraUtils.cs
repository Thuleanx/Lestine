using UnityEngine;

namespace Cinemachine {
    public static class CameraUtils {
        public static Vector3 DirectionToWorld(Vector2 movement) => DirectionToWorld(Camera.main, movement);
        public static Vector3 DirectionToWorld(Camera camera, Vector2 movement) {
            Vector3 inputDir = new Vector3(
                movement.x, 0, movement.y
            ).normalized;

            return Quaternion.Euler(
                0, Camera.main.transform.eulerAngles.y, 0f
            ) * inputDir;
        }

        public static Vector3 DirectionToWorldRelativeCamera(Camera camera, Vector3 movement, Vector3 position) {
            movement.Normalize();

            Vector3 positionInViewport = camera.WorldToViewportPoint(position);

            Ray lower = camera.ViewportPointToRay(new Vector3(positionInViewport.x, 0));
            Ray higher = camera.ViewportPointToRay(new Vector3(positionInViewport.x, 1));

            Plane xzPlane = new Plane(Vector3.up, -position.y);

            xzPlane.Raycast(lower, out float lowerDistanceFromCamera);
            xzPlane.Raycast(higher, out float higherDistanceFromCamera);

            Vector3 lowerPoint = lower.GetPoint(lowerDistanceFromCamera);
            Vector3 higherPoint = higher.GetPoint(higherDistanceFromCamera);

            Vector3 up = (higherPoint - lowerPoint).normalized;

            return movement.x * Vector3.ProjectOnPlane(camera.transform.right, Vector3.up) + movement.y * up;
        }

        public static float GetHorizontalFOVDegrees(Camera camera) {
            float verticalFOVRadians = camera.fieldOfView * Mathf.Deg2Rad;
            float horizontalFOVRadians = 2 * Mathf.Atan(Mathf.Tan(verticalFOVRadians / 2) * camera.aspect);
            float horizontalFOVDegree = Mathf.Rad2Deg * horizontalFOVRadians;
            return horizontalFOVDegree;
        }
    }
}
