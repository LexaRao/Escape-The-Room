using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody))]
public sealed class BallController : MonoBehaviour
{

    private Rigidbody rb;

    //Inventory handling
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private Texture2D rewardIcon;

    // Track if puzzle has been completed.
    private bool puzzleCompleted = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // When ball collides with the FinishZone.
    private void OnTriggerEnter(Collider other)
    {
        // Only count puzzle completed once.
        if (puzzleCompleted)
            return;

        // Check if inventoryUI is connected.
        if (inventoryUI == null)
        {
            Debug.LogError("InventoryUI is not assigned in the Inspector.");
            return;
        }

        // Mark puzzle completed and add reward Item to Inventory.
        puzzleCompleted = true;
        inventoryUI.AddItem("Cipher", rewardIcon);
    }
    
}

