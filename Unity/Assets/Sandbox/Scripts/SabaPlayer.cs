using UnityEngine;
using Scriptables;

public class SabaPlayer : MonoBehaviour {
    [SerializeField] ScriptableVector2 movementInput;
    [SerializeField] Camera mainCamera;
    [SerializeField] float speed;

    void Awake() {
        if (!mainCamera)
            mainCamera = Camera.main;
    }

    void Update() {
        Vector3 right = mainCamera.transform.right;
        Vector3 forward = mainCamera.transform.forward;
        right.y = 0;
        forward.y = 0;
        right = right.normalized;
        forward = forward.normalized;

        Vector3 desiredMoveDirection = right * movementInput.Value.x + forward * movementInput.Value.y;

        transform.position += speed * Time.deltaTime * desiredMoveDirection;
    }
}
