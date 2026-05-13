using UnityEngine;
using UnityEngine.InputSystem;

public class CipherNode : MonoBehaviour
{
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private GameObject cipherObject;

    private Camera cam; // Main camera for click event.

    private void Awake()
    {
        cam = Camera.main;
    }

    // Refrence for all placement nodes.
    private void Update()
    {
        // Check if there was a left mouse click, if not then return.
        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Vector2 mousePos = Mouse.current.position.ReadValue(); // Get postion of cursor on screen.
        Ray ray = cam.ScreenPointToRay(mousePos); // Project 2D mouse image onto 3D location.

        // Check where click landed.
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            // Get object whose collider that was clicked.
            CipherNode node = hit.collider.GetComponentInParent<CipherNode>();

            // Check if clicked node was this Object.
            if (node == this)
            {
                TryPlaceCipher(); // Try to place 3D Item.
            }
        }
    }

    // Refrence for placing down the 3D Item.
    private void TryPlaceCipher()
    {
        // Check that Inventory UI is assigned.
        if (inventoryUI == null)
        {
            Debug.LogError("InventoryUI is not assigned.");
            return;
        }

        // Check that 3D Item is assigned.
        if (cipherObject == null)
        {
            Debug.LogError("Cipher object is not assigned.");
            return;
        }

        // Check that an Item slot has been selected from the inventory.
        if (inventoryUI.SelectedItem == null)
        {
            Debug.Log("No item selected.");
            return;
        }

        // If inventory Item matches what needs to be "placed".
        if (inventoryUI.SelectedItem.itemName == "Cipher")
        {
            // Make 3D Item visible by setting it to active.
            cipherObject.SetActive(true);
            inventoryUI.ConsumeSelectedItem(); // Consume Item upon use.

            // Hide Node since Item has been placed by setting it to inactive.
            gameObject.SetActive(false); 

            Debug.Log("Cipher placed.");
        }
        else
        // Do nothing if item selected is not correct.
        {
            Debug.Log("Selected item is not the Cipher.");
        }
    }
}