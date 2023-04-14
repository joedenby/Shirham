using UnityEditor;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor.VersionControl;

[InitializeOnLoad]
public class InventoryGUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject bubble;
    [SerializeField] private GameObject window;
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
        if (InteractionFinder.main.Focus is null) return;
        if (InteractionFinder.main.Focus is not ItemObject) return;

        heldObject = InteractionFinder.main.Focus as ItemObject;
        UpdateInventory();
    }

    public void UpdateInventory() {
        var items = inventory.GetInventory;
        for (int i = 0; i < transform.childCount; i++) {
            var icon = window.transform.GetChild(i);
            var image = icon.GetChild(0).GetComponent<Image>();
            var item = items[i];

            image.sprite = item ? item.icon : null;
            icon.GetComponent<Image>().color = Color.gray;
            icon.gameObject.SetActive(i < items.Length);
        }
    }

    public void SelectIcon(int index)
    {
        var focus = InteractionFinder.main.Focus;
        if (focus is not null && focus is ItemObject) {
            var icon = window.transform.GetChild(index);
            var image = icon.GetComponent<Image>();

            LeanTween.cancel(icon.gameObject);

            // Using LeanTween.value() to modify the Image component's color directly
            LeanTween.value(icon.gameObject, image.color, Color.green, 0.5f).setOnUpdate((Color colorValue) =>
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
        Debug.Log($"Placed Obj: {heldObject?.name} [{selectedIcon}]");
        if (!heldObject) return;

        inventory.AddItem(heldObject.item, index);
        Destroy(heldObject.gameObject);
        heldObject = null;
    }
}
