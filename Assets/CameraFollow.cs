using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    public float distance = 5f;
    public float height = 2f;
    public float sensitivity = 0.2f;

    float yaw;
    float pitch;

    void Start()
    {
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void LateUpdate()
    {
        Vector2 mouse = Mouse.current.delta.ReadValue();

        yaw += mouse.x * sensitivity;
        pitch -= mouse.y * sensitivity;

        pitch = Mathf.Clamp(pitch, -40f, 70f);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        Vector3 targetPosition = player.position + Vector3.up * height;

        Vector3 cameraPosition = targetPosition - rotation * Vector3.forward * distance;

        transform.position = cameraPosition;
        transform.rotation = rotation;
    }
}