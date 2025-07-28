using UnityEngine;

public class IsometricCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float cameraDistance = 10f;
    [SerializeField] private float smoothTime = 0.8f;
    [SerializeField] private float lookAheadDistance = 1f;
    [SerializeField] private PlayerController playerController;

    private Vector3 currentVelocity;

    private void Start()
    {
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();

        if (target == null && playerController != null)
            target = playerController.transform;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Stel isometrische rotatie in
        transform.rotation = Quaternion.Euler(30, 45, 0);

        // Basis offset op basis van rotatie en afstand
        Vector3 offset = transform.rotation * new Vector3(0, 0, -cameraDistance);

        // Kijkrichting van speler (alleen XZ-vlak)
        Vector3 lookDirection = target.forward;
        lookDirection.y = 0;
        lookDirection.Normalize();

        // Look ahead offset in de richting waarin de speler kijkt
        Vector3 lookAheadOffset = lookDirection * lookAheadDistance;

        // Bereken doelpositie
        Vector3 desiredPosition = target.position + offset + lookAheadOffset;

        // Smooth naar de doelpositie
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothTime);
    }
}
