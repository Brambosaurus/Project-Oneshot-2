using UnityEngine;

public class CameraZoomOnSprint : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private float zoomOutAmount = 10f;
    [SerializeField] private float zoomSpeed = 5f;

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

        bool isSprinting = Input.GetKey(player.sprintKey);
        float targetZoom = isSprinting ? defaultZoom + zoomOutAmount : defaultZoom;

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
