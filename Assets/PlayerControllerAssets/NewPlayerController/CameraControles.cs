using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControls : MonoBehaviour
{
    [Header("Zoom Settings")]
    public Camera playerCamera;
    public float zoomSensitivity = 200f;
    public float minFOV = 30f;
    public float maxFOV = 80f;
    public float zoomSmoothTime = 0.05f;

    private float targetFOV;
    private float fovVelocity;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        targetFOV = playerCamera.fieldOfView;
    }

    void Update()
    {
        playerCamera.fieldOfView = Mathf.SmoothDamp(
            playerCamera.fieldOfView,
            targetFOV,
            ref fovVelocity,
            zoomSmoothTime
        );
    }

    // Called by Input System when mouse scroll input occurs
    public void OnZoom(InputValue value)
    {
        Vector2 scrollDelta = value.Get<Vector2>();
        float scrollY = scrollDelta.y;

        targetFOV -= scrollY * zoomSensitivity * Time.deltaTime;
        targetFOV = Mathf.Clamp(targetFOV, minFOV, maxFOV);
    }
}
