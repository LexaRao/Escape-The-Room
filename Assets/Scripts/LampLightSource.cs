using UnityEngine;

[RequireComponent(typeof(Light))]
public class LampLightSource : MonoBehaviour
{
    [Header("Lamp Position")]
<<<<<<< HEAD
<<<<<<< HEAD
    public float heightAboveGround = 0f;
=======
    public float heightAboveGround = 3f;
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
=======
    public float heightAboveGround = 3f;
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
    public LayerMask groundLayer = ~0;

    [Header("Light Settings")]
    public float lightRadius = 6f;
    public float lightIntensity = 2f;
    public Color lightColor = Color.white;

    [Header("Optional Visual Range")]
    public bool showRadiusInEditor = true;

    private Light lampLight;

    void Awake()
    {
        lampLight = GetComponent<Light>();
        SetupLight();
<<<<<<< HEAD
<<<<<<< HEAD
        // SetHeightAboveGround();
=======
        SetHeightAboveGround();
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
=======
        SetHeightAboveGround();
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
    }

    void OnValidate()
    {
        lampLight = GetComponent<Light>();

        if (lampLight != null)
        {
            SetupLight();
<<<<<<< HEAD
<<<<<<< HEAD
            // SetHeightAboveGround();
=======
            SetHeightAboveGround();
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
=======
            SetHeightAboveGround();
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
        }
    }

    private void SetupLight()
    {
        lampLight.type = LightType.Point;
        lampLight.range = lightRadius;
        lampLight.intensity = lightIntensity;
        lampLight.color = lightColor;
        lampLight.shadows = LightShadows.Soft;
    }

    private void SetHeightAboveGround()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 10f, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            transform.position = new Vector3(
                transform.position.x,
                hit.point.y + heightAboveGround,
                transform.position.z
            );
        }
        else
        {
            transform.position = new Vector3(
                transform.position.x,
                heightAboveGround,
                transform.position.z
            );
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!showRadiusInEditor)
            return;

        Gizmos.color = lightColor;
        Gizmos.DrawWireSphere(transform.position, lightRadius);

        Gizmos.DrawLine(
            transform.position,
            transform.position + Vector3.down * heightAboveGround
        );
    }
}