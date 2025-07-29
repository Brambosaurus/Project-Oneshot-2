using UnityEngine;
using System.Collections;

public class CameraZoomController : MonoBehaviour
{
    private Vector3 originalPos;

    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeMagnitude = 0.3f;

    public void TriggerShake()
    {
        originalPos = transform.localPosition;
        StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        float elapsed = 0.0f;

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
}
