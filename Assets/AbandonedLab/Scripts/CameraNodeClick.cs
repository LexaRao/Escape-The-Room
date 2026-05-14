using UnityEngine;

public class CameraNodeClick : MonoBehaviour
{
    [SerializeField] private GameObject endMenu;
    [SerializeField] private float delay = 3f;

    private bool hasBeenClicked = false;

    void Update()
    {
        if (hasBeenClicked) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)
                {
                    hasBeenClicked = true;
                    StartCoroutine(ActivateEndMenuAfterDelay());
                }
            }
        }
    }

    private System.Collections.IEnumerator ActivateEndMenuAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        if (endMenu != null)
        {
            endMenu.SetActive(true);
        }
        else
        {
            Debug.LogWarning("End Menu not assigned!");
        }
    }
}