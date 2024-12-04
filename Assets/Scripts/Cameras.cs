using UnityEngine;

public class FixedCameraWithZoom : MonoBehaviour
{
    public Transform player; // The player character to follow
    public Vector3 offset = new Vector3(0, 10, -5); // Offset relative to the player
    public float smoothSpeed = 0.125f; // Smoothing factor for camera movement
    public float zoomSpeed = 2f; // Speed of zooming with the mouse wheel
    public float minZoom = 5f; // Minimum zoom distance
    public float maxZoom = 20f; // Maximum zoom distance
    public Vector3 fixedRotation = new Vector3(45f, 0f, 0f); // Desired fixed rotation in degrees

    private float currentZoom;

    void Start()
    {
        // Initialize current zoom to the magnitude of the offset
        currentZoom = offset.magnitude;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Handle zoom input
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scrollInput * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // Update the offset based on the current zoom level
        offset = offset.normalized * currentZoom;

        // Smoothly follow the player
        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Lock the rotation to the fixed values
        transform.rotation = Quaternion.Euler(fixedRotation);
    }
}
