using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    // Class for tracking Items to be put in the Inventory.
    [System.Serializable]
    public class InventoryEntry
    {
        public string itemName;
        public Texture2D icon;
    }

    private Button[] slots; // Array of Inventory Slots.
    private bool[] occupied; // Boolean for checking if Inventory Slot has been filled.
    private InventoryEntry[] items; // Array of Inventory Items that have been passed in.

    private VisualElement inventoryItemImage; // Image attached to cursor.
    private Texture2D selectedIcon; // Passed in png image of Inventory Item.

    private bool isHoldingItem = false; // Boolean to check if an item is currently being selected.
    private int selectedIndex = -1; // Index of slot that has been selected.

    private InventoryEntry selectedItem; // Current Item being selected.
    public InventoryEntry SelectedItem => selectedItem; // Same as getSelectedItem(), public access to selected item without ability to change it.

    private void Awake()
    {
        UIDocument document = GetComponent<UIDocument>();
        VisualElement root = document.rootVisualElement;

        // Array of four Inventory Slots.
        slots = new Button[]
        {
            root.Q<Button>("SlotOne"),
            root.Q<Button>("SlotTwo"),
            root.Q<Button>("SlotThree"),
            root.Q<Button>("SlotFour")
        };

        // Arrays to track occupied inventory slotes.
        occupied = new bool[slots.Length];
        items = new InventoryEntry[slots.Length];

        // Create a visual element that the cursor is "holding".
        inventoryItemImage = new VisualElement();
        inventoryItemImage.style.width = 100; // Size in pixels.
        inventoryItemImage.style.height = 100;
        inventoryItemImage.style.position = Position.Absolute; // Enable image to be freely positioned.
        inventoryItemImage.style.display = DisplayStyle.None; // Hide element by default.
        inventoryItemImage.pickingMode = PickingMode.Ignore; // Make sure image does not interfere with cursor input.
        inventoryItemImage.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain); // 

        root.Add(inventoryItemImage); // Add inventoryItemImage to Inventory UI root.

        // For all Items in slots.
        for (int i = 0; i < slots.Length; i++)
        {
            int index = i; // Store current index.

            // Check that all slots are registering correctly.
            if (slots[i] == null)
            {
                Debug.LogError($"Slot {i} was not found.");
                continue;
            }

            // Make sure all slots only take "mouse click" input.
            slots[i].focusable = false;
            slots[i].tabIndex = -1;

            // Make sure image stays within InventoryUI bounds.
            slots[i].style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);

            // When slot is clicked, call OnSlotClicked for the current index.
            slots[i].RegisterCallback<ClickEvent>(evt => OnSlotClicked(index, evt));
        }

    }

    // Make sure selected Item's icon is always centered to the cursor.
    private void Update()
    {
        // Pass if Item is not selected.
        if (!isHoldingItem || Mouse.current == null)
            return;

        Vector2 pos = Mouse.current.position.ReadValue(); // Get mouse position outside of UI Toolkit.

        // Center icon to cursor.
        inventoryItemImage.style.left = pos.x - 50;
        inventoryItemImage.style.top = Screen.height - pos.y - 50;
    }

    // Adds Item image to first available Inventory slots.
    // Called by puzzle upon completion.
    public void AddItem(string itemName, Texture2D itemIcon)
    {
        // Check that there was an image passed in for the Item.
        if (string.IsNullOrEmpty(itemName) || itemIcon == null)
        {
            Debug.LogError("Item icon/name is missing.");
            return;
        }

        // Go through whole array.
        for (int i = 0; i < slots.Length; i++)
        {
            // First slot that is not occupied by an Item.
            if (!occupied[i])
            {
                // Add Item information to array of items.
                items[i] = new InventoryEntry
                {
                    itemName = itemName,
                    icon = itemIcon
                };

                // Set image of slot to be that of the Item occupying it.
                slots[i].style.backgroundImage = new StyleBackground(itemIcon); 
                // Make sure image fits inside slot without streching.
                slots[i].style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
                occupied[i] = true; // Mark index as being occupied.

                return;
            }
        }

        Debug.Log("Inventory full.");
    }

    // When a slot is clicked, show the item on the cursor to indicate it has been selected.
    private void OnSlotClicked(int index, ClickEvent evt)
    {
        // If nothing is there do nothing.
        if (!occupied[index])
            return;
        // If item has already been selected, deselect it.
        if (isHoldingItem && selectedIndex == index)
        {
            ClearSelectedItem();
            return;
        }

        // Store current index as selected item and set cursor to holding an Item.
        selectedIndex = index;
        isHoldingItem = true;

        // Store name and image of the Item in the selected slot.
        selectedItem = items[index];
        selectedIcon = selectedItem.icon;

        inventoryItemImage.style.backgroundImage = new StyleBackground(selectedIcon);
        inventoryItemImage.style.display = DisplayStyle.Flex;

        // Center icon on cursor immediately
        inventoryItemImage.style.left = evt.position.x - 50;
        inventoryItemImage.style.top = evt.position.y - 50;
    }

    // Deselect Item and remove image from cursor.
    public void ClearSelectedItem()
    {
        // Deselect Item.
        isHoldingItem = false; 
        selectedIndex = -1;
        selectedItem = null;
        selectedIcon = null;

        // Remove image from cursor.
        inventoryItemImage.style.display = DisplayStyle.None;
    }

    // Remove the currently selected item from the inventory.
    public void ConsumeSelectedItem()
    {
        // If no index selected return nothing.
        if (selectedIndex < 0)
            return;

        // Remove Item from index.
        items[selectedIndex] = null;
        occupied[selectedIndex] = false;

        // Remove image from Inventory slot.
        slots[selectedIndex].style.backgroundImage = null;

        ClearSelectedItem();
    }
}


