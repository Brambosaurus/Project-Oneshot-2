using UnityEngine;

public class CameraFollowTilt : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Vector3 offset = new Vector3(0f, 10f, -10f);
    [SerializeField] private float followSpeed = 5f;

    [Header("Rotation Settings")]
    [SerializeField] private float maxYawOffset = 20f; // hoe schuin draaien we horizontaal
    [SerializeField] private float rotationLerpSpeed = 3f;

    private void LateUpdate()
    {
        if (player == null || playerController == null) return;

        // === POSITIE ===
        Vector3 targetPosition = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);

        // === ROTATIE ===
        Vector3 moveDir = playerController.movementDirection;
        if (moveDir.sqrMagnitude > 0.01f)
        {
            float sprintFactor = playerController.IsSprinting ? 1.2f : 0.6f;

            // Bereken een lichte yaw (horizontale rotatie) in de richting van de beweging
            Quaternion desiredRotation = Quaternion.LookRotation(-offset.normalized); // baseline
            float angleOffset = maxYawOffset * sprintFactor;
            float signedAngle = Vector3.SignedAngle(Vector3.forward, moveDir, Vector3.up);
            float yawOffset = Mathf.Clamp(signedAngle, -angleOffset, angleOffset);

            Quaternion tiltRotation = Quaternion.Euler(0f, yawOffset, 0f);
            Quaternion finalRotation = desiredRotation * tiltRotation;

            transform.rotation = Quaternion.Lerp(transform.rotation, finalRotation, Time.deltaTime * rotationLerpSpeed);
        }
        else
        {
            // Terug naar standaard wanneer speler stilstaat
            Quaternion defaultRot = Quaternion.LookRotation(-offset.normalized);
            transform.rotation = Quaternion.Lerp(transform.rotation, defaultRot, Time.deltaTime * rotationLerpSpeed);
        }
    }
}
