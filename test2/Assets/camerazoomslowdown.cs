using UnityEngine;

public class CameraZoomOnSlowdown : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float zoomInAmount = 10f; // kleiner = meer zoom

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

        // Als terrein vertraging actief is (<1), zoom in
        if (player.TerrainSpeedMultiplier < 1f)
        {
            targetZoom -= zoomInAmount;
        }

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