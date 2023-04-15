using UnityEditor;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

[InitializeOnLoad]
public class InventoryGUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject bubble;
    [SerializeField] private GameObject window;
    [SerializeField] private ItemIcon[] icons;
    public static ItemObject heldObject;
    
    private int selectedIcon = -1;
    private bool isDraggingIcon => icons.Any(x => x.isBeingDragged);
    public static InventoryGUI activeWindow { get; private set; }



    private void Update()
    {
        if (Input.GetMouseButtonUp(0)) {
            SetItem(selectedIcon);
        }
    }

    public void ShowInventory(bool active) {
        activeWindow = active ? this : null;    //Set active window based on current pointer location
        if (!active && isDraggingIcon) return;  //Inventory stay open on drag check

        window.SetActive(active);
        bubble.SetActive(!active);
        selectedIcon = -1;


        if (!active) return;
        UpdateInventory();

        if (InteractionFinder.main.Focus is null) return;
        if (InteractionFinder.main.Focus is not ItemObject) return;

        heldObject = InteractionFinder.main.Focus as ItemObject;
    }

    public void UpdateInventory() {
        var items = inventory.GetInventory;
        for (int i = 0; i < icons.Length; i++) {
            if (i >= items.Length) return;
            icons[i].AssignItem(items[i]);
            icons[i].gameObject.SetActive(i < items.Length);
        }
    }

    public void SelectIcon(int index) {
        icons[index].SetSelected(heldObject, true);
        selectedIcon = index;
    }

    public void DeSelectIcon(int index) {
        icons[index].SetSelected(heldObject, false);
        selectedIcon = -1;
    }

    public void SetItem(int index) {
        if (!heldObject) return;

        if (!inventory.ItemAtSlot(index)) {
            inventory.AddItem(heldObject.item, index);
        }
        else if (inventory.HasFreeSlot(out int slotIndex)) {
            inventory.MoveItem(index, slotIndex);
            inventory.AddItem(heldObject.item, index);
        }
        else { 
            //Drop Item in slot on floor and add current
        }
        
        Destroy(heldObject.gameObject);
        heldObject = null;
    }

}
