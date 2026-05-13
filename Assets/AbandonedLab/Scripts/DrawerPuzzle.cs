using UnityEngine;

public class DrawerPuzzle : PuzzleInteractable
{
    [Header("Drawer Settings")]
    public Transform drawerTransform;
    public GameObject keyCardObject;
    public float slideDistance = 0.5f;
    public float slideSpeed = 2f;

    private bool isOpen = false;
    private bool isSliding = false;
    private Vector3 closedPosition;
    private Vector3 openPosition;

    void Start()
    {
        if (drawerTransform != null)
        {
            closedPosition = drawerTransform.localPosition;
            openPosition = closedPosition + new Vector3(0, 0, -slideDistance);
        }

        if (keyCardObject != null)
            keyCardObject.SetActive(false);
    }

    // Override instead of redefine — calls parent E key detection too
    protected override void Update()
    {
        base.Update();

        if (isSliding && drawerTransform != null)
        {
            drawerTransform.localPosition = Vector3.Lerp(
                drawerTransform.localPosition,
                openPosition,
                Time.deltaTime * slideSpeed
            );

            if (Vector3.Distance(drawerTransform.localPosition, openPosition) < 0.01f)
            {
                drawerTransform.localPosition = openPosition;
                isSliding = false;
                OnDrawerFullyOpen();
            }
        }
    }

    public override void Interact()
    {
        if (isOpen) return;
        isOpen = true;
        isSliding = true;
        Debug.Log("Drawer opened!");
    }

    private void OnDrawerFullyOpen()
    {
        if (keyCardObject != null)
        {
            keyCardObject.SetActive(true);
            // Start float animation
            StartCoroutine(FloatKeyCard());
        }
        Debug.Log("Keycard revealed!");
        MarkSolved();
    }

    private System.Collections.IEnumerator FloatKeyCard()
    {
        Vector3 startPos = keyCardObject.transform.localPosition;
        Vector3 targetPos = startPos + new Vector3(0, 0.3f, 0);
        float duration = 0.8f;
        float elapsed = 0f;

        // Rise up
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            keyCardObject.transform.localPosition = 
                Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        // Gentle bob
        while (true)
        {
            float bob = Mathf.Sin(Time.time * 2f) * 0.05f;
            keyCardObject.transform.localPosition = 
                targetPos + new Vector3(0, bob, 0);
            yield return null;
        }
    }
}
