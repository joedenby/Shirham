using UnityEditor;
using UnityEngine;
using System.Linq;

/**
 * ==================================  InventoryGUI.cs  =============================================
 * The InventoryGUI class is responsible for managing the graphical user interface (GUI) of a game's 
 * inventory system. The class handles showing and hiding the inventory window, updating the inventory 
 * icons, selecting and deselecting icons, and adding items to the inventory.
 */
[InitializeOnLoad]
public class InventoryGUI : MonoBehaviour
{
    // Serialize private fields to make them accessible in Unity editor
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject bubble;
    [SerializeField] private GameObject window;
    [SerializeField] private ItemIcon[] icons;

    // Declare public static field to store the currently held object
    public static ItemObject heldObject;

    // Create properties for the selected icon and the active window
    public int selectedIcon { get; private set; }
    private bool isDraggingIcon => icons.Any(x => x.isBeingDragged);
    public static InventoryGUI activeWindow { get; private set; }


    // Update method runs once per frame
    private void Update()  {
        // Return if there is no active window or if it doesn't match the current instance
        if (!activeWindow || !activeWindow.Equals(this)) return;

        // Set the item if the left mouse button is released and an icon is selected
        if (Input.GetMouseButtonUp(0) && selectedIcon >= 0)
        {
            SetItem(selectedIcon);
        }
    }

    // Show or hide the inventory
    public void ShowInventory(bool active)  {
        // Set active window based on current pointer location
        activeWindow = active ? this : null;

        // Inventory stay open on drag check
        if (!active && isDraggingIcon) return;

        // Show or hide the inventory window and bubble
        window.SetActive(active);
        bubble.SetActive(!active);

        // Reset the selected icon
        selectedIcon = -1;

        // Update the inventory if it is active
        if (!active) return;
        UpdateInventory();

        // Check if the focused object is an ItemObject, and set it as the heldObject
        if (InteractionFinder.main.Focus is null) return;
        if (InteractionFinder.main.Focus is not ItemObject) return;
        heldObject = InteractionFinder.main.Focus as ItemObject;
    }

    // Update the inventory's icons with the current items
    public void UpdateInventory()
    {
        var items = inventory.GetInventory;
        for (int i = 0; i < icons.Length; i++)
        {
            if (i >= items.Length) return;
            icons[i].AssignItem(items[i]);
            icons[i].gameObject.SetActive(i < items.Length);
        }
    }

    // Select an inventory icon
    public void SelectIcon(int index)
    {
        selectedIcon = index;
        icons[index].SetSelected(heldObject, true);
    }

    // Deselect an inventory icon
    public void DeSelectIcon(int index)
    {
        selectedIcon = -1;
        icons[index].SetSelected(heldObject, false);
    }

    // Set the item at the specified index
    public void SetItem(int index)
    {
        // Return if there is no held object or the index is invalid
        if (!heldObject || index < 0) return;

        // Add the held object to the inventory or move it to a free slot
        if (!inventory.ItemAtSlot(index))
        {
            inventory.AddItem(heldObject.item, index);
        }
        else if (inventory.HasFreeSlot(out int slotIndex))
        {
            inventory.MoveItem(index, slotIndex);
            inventory.AddItem(heldObject.item, index);
        }
        else
        {
            // Drop the item in the slot on the floor and add the current held object
            // Implement your logic to drop the item in the world here
        }

        // Destroy the held object's game object and set heldObject to null
        Destroy(heldObject.gameObject);
        heldObject = null;
    }
}