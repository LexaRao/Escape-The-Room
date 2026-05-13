using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    [Header("Flicker Settings")]
    public Light targetLight;
    public float minIntensity = 0.4f;
    public float maxIntensity = 1.2f;
    public float flickerSpeed = 0.05f;
    public float normalIntensity = 0.8f;

    [Header("Flicker Frequency")]
    public float flickerChance = 0.05f; // 5% chance per frame to flicker

    private float nextFlickerTime;
    private bool isFlickering = false;

    void Start()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light>();

        targetLight.intensity = normalIntensity;
    }

    void Update()
    {
        if (Time.time >= nextFlickerTime)
        {
            if (Random.value < flickerChance)
                StartFlicker();
        }

        if (isFlickering)
        {
            targetLight.intensity = Random.Range(minIntensity, maxIntensity);

            if (Time.time >= nextFlickerTime)
            {
                isFlickering = false;
                targetLight.intensity = normalIntensity;
                // Random pause before next flicker
                nextFlickerTime = Time.time + Random.Range(2f, 8f);
            }
        }
    }

    private void StartFlicker()
    {
        isFlickering = true;
        // Flicker lasts 0.1 to 0.5 seconds
        nextFlickerTime = Time.time + Random.Range(0.1f, 0.5f);
    }
}
