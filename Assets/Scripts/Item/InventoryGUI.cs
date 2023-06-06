using GameManager;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

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

    // Create properties for the selected icon and the active window
    public int selectedIcon { get; private set; }
    private bool isDraggingIcon => icons.Any(x => x.isBeingDragged);
    public static InventoryGUI activeWindow { get; private set; }
    private bool isActiveWindow => activeWindow && activeWindow.Equals(this);
    public static bool show => Inventory.heldObject || InputManager.PlayerInput.HUD.IsPressed();


    private void Awake()   {
        if (inventory) return;

        inventory = GetComponentInParent<Inventory>();
        if (!inventory) {
            Debug.LogWarning($"Could not find inventroy for {name}");
            return;
        }

        inventory.onInventoryUpdate.AddListener(UpdateInventory);
        UpdateInventory();
    }

    // Update method runs once per frame
    private void Update()  {
        bubble.SetActive(show && !HasPermission(Inventory.InventoryRights.Disabled));
        transform.localScale = new Vector2(inventory.transform.localScale.x > 0 ? 1 : -1, 1);

        // Return if there is no active window or if it doesn't match the current instance
        if (!activeWindow || !activeWindow.Equals(this)) {
            return;
        }
        
        // Set the item if the left mouse button is released and an icon is selected
        if (Input.GetMouseButtonUp(0) && selectedIcon >= 0) {
            SetItem(selectedIcon);
        }
    }

    private bool HasPermission(Inventory.InventoryRights inventoryRight) {
        return inventory?.permissions == inventoryRight;
    }

    // Show or hide the inventory
    public void ShowInventory(bool active)  {
        // Set active window based on current pointer location
        activeWindow = active ? this : null;

        // Inventory stay closed if no permission
        if (active && HasPermission(Inventory.InventoryRights.Disabled)) return;

        // Show or hide the inventory window and bubble
        window.SetActive(active);

        // Reset the selected icon
        selectedIcon = -1;

        // Update the inventory if it is active
        if (!active) return;

        UpdateInventory();
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
            icons[i].GetComponent<Image>().enabled = true;
            icons[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
        }
    }

    // Select an inventory icon
    public void SelectIcon(int index)
    {
        selectedIcon = index;
        icons[index].SetSelected(Inventory.heldObject, true);
    }

    // Deselect an inventory icon
    public void DeSelectIcon(int index)
    {
        selectedIcon = -1;
        icons[index].SetSelected(Inventory.heldObject, false);
    }

    // Set the item at the specified index
    public void SetItem(int index)
    {
        // Return if there is no held object or the index is invalid
        if (!Inventory.heldObject || index < 0) return;

        // Add the held object to the inventory or move it to a free slot
        if (!inventory.ItemAtSlot(index)) {
            inventory.AddItem(Inventory.heldObject.item, index);
        }
        else if (inventory.HasFreeSlot(out int slotIndex)) {
            inventory.MoveItem(index, slotIndex);
            inventory.AddItem(Inventory.heldObject.item, index);
        } 

        // Destroy the held object's game object and set heldObject to null
        Destroy(Inventory.heldObject.gameObject);
        Inventory.heldObject = null;
    }

}