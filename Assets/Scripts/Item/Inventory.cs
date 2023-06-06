using UnityEngine;
using UnityEngine.Events;


/*
 * ==================================== Inventory.cs =============================================
 * The Inventory class is responsible for managing a game character's inventory, including adding and 
 * removing items, changing the inventory size, and moving items within the inventory. The class stores 
 * the inventory items in an array, raises events when the inventory is updated, and provides methods 
 * for various inventory operations.
 */
public class Inventory : MonoBehaviour
{
    [Header("Configuration")]
    // Array of items in the inventory, default size is 6
    [SerializeField] private Item[] items = new Item[6];
    // Flag to set the inventory as large or small
    [SerializeField] private bool isLarge = false;

    // Flag to set the inventory as open or closed
    public InventoryRights permissions = InventoryRights.AddOnly;
    public enum InventoryRights { 
        Full,       // Can add and remove items
        AddOnly,    // Can only add items
        RemoveOnly, // Can only remove items
        Disabled    // Cannot add or remove items
    }

    // Declare public static field to store the currently held object
    public static ItemObject heldObject;

    // Public getter to return the items in the inventory
    public Item[] GetInventory => items;

    [Header("Events")]
    // UnityEvent to notify when the inventory has been updated
    public UnityEvent onInventoryUpdate = new UnityEvent();
    public static UnityEvent onItemPickUp = new UnityEvent();
    public static UnityEvent onItemDrop = new UnityEvent();



    // Called when the script is loaded or a value is changed in the Inspector
    private void OnValidate() {
        MakeLarge(isLarge);
    }

    // Update the inventory size and rearrange the items based on the "enabled" parameter
    public void MakeLarge(bool enabled) {
        isLarge = enabled;
        var newItems = new Item[isLarge ? 12 : 6];
        for (int i = 0; i < items.Length; i++)
        {
            if (i >= newItems.Length)
            {
                //TODO: Add Drop Item Logic
                continue;
            }
            newItems[i] = items[i];
        }
        items = newItems;
        onInventoryUpdate.Invoke();
    }

    // Add an item to the inventory at a specific index, or find the first available slot
    public void AddItem(Item item, int index = -1)  {
        // Is item being added outside of inventory?
        if (index == -1)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] is null)
                {
                    items[i] = item;
                    onInventoryUpdate?.Invoke();
                    return;
                }
            }
            //Drop Item on floor
            return;
        }

        // User selected slot
        if (items[index] is null)
        {
            items[index] = item;
            onInventoryUpdate?.Invoke();
            return;
        }

        // Find first free slot
        if (HasFreeSlot(out int freeSlot))
        {
            items[freeSlot] = item;
            onInventoryUpdate.Invoke();
            return;
        }
    }

    // Remove an item from the inventory at the specified index
    public void RemoveItem(int index)  {
        items[index] = null;
        onInventoryUpdate.Invoke();
    }

    // Check if there's an item at the specified index
    public bool ItemAtSlot(int index) => items[index] is not null;

    // Move an item from one slot to another in the inventory
    public void MoveItem(int a, int b)  {
        var itemA = items[a];
        items[a] = items[b];
        items[b] = itemA;
    }

    // Check if the inventory has a free slot and return the index of the first available slot
    public bool HasFreeSlot(out int index)  {
        index = -1;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] is null)
            {
                index = i;
                return true;
            }
        }

        return false;
    }
}