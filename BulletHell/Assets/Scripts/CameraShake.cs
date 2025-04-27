using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float shakeDuration = 0f;
    public float shakeMagnitude = 0.1f;
    public float shakeFrequency = 0.1f;

    private Vector3 originalPos;
    private Quaternion originalRot;

    void Start()
    {
        originalPos = transform.localPosition;
        originalRot = transform.localRotation;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            shakeDuration -= Time.deltaTime;

            Vector3 shakeOffset = Random.insideUnitSphere * shakeMagnitude;

            Quaternion shakeRotation = Random.rotation;

            transform.localPosition = originalPos + shakeOffset;
            transform.localRotation = Quaternion.Slerp(originalRot, shakeRotation, shakeFrequency * Time.deltaTime);
        }
        else
        {
            transform.localPosition = originalPos;
            transform.localRotation = originalRot;
        }
    }

    // Call this function to start the shake
    public void Shake(float duration, float magnitude, float frequency = 0.1f)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
        shakeFrequency = frequency;
    }
}
