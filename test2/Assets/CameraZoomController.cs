using UnityEngine;
using System.Collections;

public class CameraZoomController : MonoBehaviour
{
    private Vector3 originalLocalPosition;

    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeMagnitude = 0.3f;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 5f;

    private Camera cam;
    private float defaultZoom;
    private bool isZooming = false;
    private float targetZoom;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam.orthographic)
        {
            defaultZoom = cam.orthographicSize;
        }
        else
        {
            defaultZoom = cam.fieldOfView;
        }

        originalLocalPosition = transform.localPosition;
    }

    private void Update()
    {
        if (isZooming)
        {
            if (cam.orthographic)
            {
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
            }
            else
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetZoom, Time.deltaTime * zoomSpeed);
            }
        }
    }

    public void TriggerShake()
    {
        StopAllCoroutines(); // stop shake or zoom before starting a new one
        StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        float elapsed = 0.0f;
        Vector3 originalPos = transform.localPosition;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.localPosition = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }

    public void TriggerZoom(float zoomAmount, float duration)
    {
        StopCoroutine("ZoomRoutine");
        StartCoroutine(ZoomRoutine(zoomAmount, duration));
    }

    private IEnumerator ZoomRoutine(float zoomAmount, float duration)
    {
        isZooming = true;

        if (cam.orthographic)
            targetZoom = defaultZoom - zoomAmount;
        else
            targetZoom = defaultZoom - zoomAmount;

        yield return new WaitForSeconds(duration);

        ResetZoom();
    }

    public void ResetZoom()
    {
        targetZoom = defaultZoom;
        isZooming = true;
    }
}
