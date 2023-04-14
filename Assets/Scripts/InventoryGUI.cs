using UnityEditor;
using UnityEngine.UI;
using UnityEngine;

[InitializeOnLoad]
public class InventoryGUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject bubble;
    [SerializeField] private GameObject window;
    [SerializeField] private Sprite defaultItemIcon;
    public static ItemObject heldObject { get; private set; }
    private int selectedIcon = -1;



    private void Update()
    {
        if (Input.GetMouseButtonUp(0)) {
            SetItem(selectedIcon);
        }
    }

    public void ShowInventory(bool active) {
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
        for (int i = 0; i < window.transform.childCount; i++) {
            var icon = window.transform.GetChild(i);
            var image = icon.GetChild(0).GetComponent<Image>();
            var item = i < items.Length ? items[i] : null;

            image.sprite = item ? item.icon : defaultItemIcon;
            icon.GetComponent<Image>().color = Color.gray;
            icon.gameObject.SetActive(i < items.Length);
        }
    }

    public void SelectIcon(int index)
    {
        if (heldObject || inventory.ItemAtSlot(index)) {
            var icon = window.transform.GetChild(index);
            var image = icon.GetComponent<Image>();

            LeanTween.cancel(icon.gameObject);
            Color selectionColor = heldObject && inventory.ItemAtSlot(index) ? Color.red : Color.green;

            // Using LeanTween.value() to modify the Image component's color directly
            LeanTween.value(icon.gameObject, image.color, selectionColor, 0.5f).setOnUpdate((Color colorValue) =>
            {
                image.color = colorValue;
            });
        }

        selectedIcon = index;
    }

    public void DeSelectIcon(int index) {
        var icon = window.transform.GetChild(index);
        var image = icon.GetComponent<Image>();

        LeanTween.cancel(icon.gameObject);

        // Using LeanTween.value() to modify the Image component's color directly
        LeanTween.value(icon.gameObject, image.color, Color.grey, 0.5f).setOnUpdate((Color colorValue) =>
        {
            image.color = colorValue;
        });

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
