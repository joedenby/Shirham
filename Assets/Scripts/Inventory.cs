using UnityEngine;
using UnityEngine.Events;   

public class Inventory : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private Item[] items = new Item[6];
    [SerializeField] private bool isLarge = false;

    public Item[] GetInventory => items;
    
    [Header("Events")]
    public UnityEvent onInventoryUpdate = new UnityEvent();


    public Inventory() {
        items = new Item[isLarge ? 12 : 6]; 
    }

    public void MakeLarge(bool enabled) {
        isLarge = enabled;
        var newItems = new Item[isLarge ? 12 : 6];
        for (int i = 0; i < items.Length; i++) {
            if(i >= newItems.Length) {
                //TODO: Add Drop Item Logic
                continue;
            }
            newItems[i] = items[i];
        }
        items = newItems;
        onInventoryUpdate.Invoke();
    }

    public void AddItem(Item item, int index = -1) {
        //Is item being added outside of inventory?
        if(index == -1) {
            for (int i = 0; i < items.Length; i++) {
                if (items[i] is null) {
                    items[i] = item;
                    onInventoryUpdate?.Invoke();
                    return;
                }
            }
            //Drop Item on floor
            return;
        }
        
        //User selected slot
        if(items[index] is null) {
            items[index] = item;
            onInventoryUpdate?.Invoke();
            return;
        }

        //Find first free slot
        if(HasFreeSlot(out int freeSlot)) {
            items[freeSlot] = item;
            onInventoryUpdate.Invoke();
            return;
        }
        
        //TODO: Drop Item on floor

    }

    public void RemoveItem(int index) {
        items[index] = null;
        onInventoryUpdate.Invoke();
    }

    public bool ItemAtSlot(int index) => items[index] is not null;

    public void MoveItem(int a, int b) {
        var itemA = items[a];
        items[a] = items[b];
        items[b] = itemA;
    }

    public bool HasFreeSlot(out int index) {
        index = -1;
        for (int i = 0; i < items.Length; i++) {
            if (items[i] is null) {
                index = i;
                return true;
            }
        }

        return false;
    }
}

