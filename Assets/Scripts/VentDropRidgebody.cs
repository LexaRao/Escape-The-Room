using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VentDropRigidbody : MonoBehaviour
{
    [Header("Drop Settings")]
    public float doubleClickTime = 0.25f;
    public float stopY = 0f;
    public float fallDrag = 0.5f;

    private float lastClickTime = 0f;
    private Rigidbody rb;
    private bool isDropping = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.isKinematic = true;

        // Ensure the root has a collider
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            MeshCollider mc = gameObject.AddComponent<MeshCollider>();
            mc.convex = true;
        }
        else if (col is MeshCollider meshCol && !meshCol.convex)
        {
            meshCol.convex = true;
        }
    }

    void Update()
    {
        DetectDoubleClick();
        StopAtGround();
    }

    void DetectDoubleClick()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            // FIX: Works for ANY child mesh
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                float timeSinceLastClick = Time.time - lastClickTime;

                if (timeSinceLastClick <= doubleClickTime && !isDropping)
                    StartDrop();

                lastClickTime = Time.time;
            }
        }
    }

    void StartDrop()
    {
        isDropping = true;

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearDamping = fallDrag;
    }

    void StopAtGround()
    {
        if (!isDropping)
            return;

        if (transform.position.y <= stopY)
        {
            Vector3 pos = transform.position;
            pos.y = stopY;
            transform.position = pos;

            rb.linearVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = true;

            isDropping = false;
        }
    }
}