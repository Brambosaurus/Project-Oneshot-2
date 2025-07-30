using UnityEngine;

public class CameraZoomOnSlowdown : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private float zoomSpeed = 5f;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomInAmount = 10f;   // kleiner = meer inzoomen
    [SerializeField] private float zoomOutAmount = 5f;   // groter = meer uitzoomen

    private Camera cam;
    private float defaultZoom;
    private bool isOrtho;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        isOrtho = cam.orthographic;

        if (isOrtho)
            defaultZoom = cam.orthographicSize;
        else
            defaultZoom = cam.fieldOfView;
    }

    private void LateUpdate()
    {
        if (player == null) return;

        float targetZoom = defaultZoom;

        // Zoom in bij trage zones zoals modder
        if (player.TerrainSpeedMultiplier < 1f)
        {
            targetZoom -= zoomInAmount;
        }
        // Zoom uit bij sprinten (enkel als niet vertraagd)
        else if (player.IsSprinting)
        {
            targetZoom += zoomOutAmount;
        }

        // Smooth interpolatie tussen huidige en target zoom
        if (isOrtho)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
        }
        else
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetZoom, Time.deltaTime * zoomSpeed);
        }
    }
}
