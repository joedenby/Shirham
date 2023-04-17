using UnityEngine;
using TMPro;
using System.Drawing.Text;
using System;

public class DebugInventoryStuff : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI debugText;
    [SerializeField] UnitController debugUnit;

    private Inventory inventory;
    private InventoryGUI inventoryGUI;

    private void Update()
    {
        if(!debugUnit) return;
        UpdateDebug();
    }

    private void OnValidate()
    {
        try {
            inventory = debugUnit.GetComponent<Inventory>();
            inventoryGUI = debugUnit.GetComponentInChildren<InventoryGUI>();
            UpdateDebug();
        }  catch(NullReferenceException ex) { 
            Debug.LogError(ex.Message);
        }

    }

    private void UpdateDebug()
    {
        var unit = InventoryGUI.activeWindow ? InventoryGUI.activeWindow.GetComponentInParent<UnitController>() : debugUnit;
        Inventory unitInventory = InventoryGUI.activeWindow ? unit.GetComponentInParent<Inventory>() : inventory;
        InventoryGUI gui = InventoryGUI.activeWindow ? InventoryGUI.activeWindow : inventoryGUI;

        debugText.text = $"Unit: {unit.name}";
        debugText.text += "\nInventory: ";
        foreach(var item in unitInventory.GetInventory)
        {
            debugText.text += $"\n  [{(item ? item.name : "<empty>")}]";
        }

        if(InventoryGUI.activeWindow)
            debugText.text += $"\nActive Window: {InventoryGUI.activeWindow.GetComponentInParent<UnitController>().name}'s Inventory";

        debugText.text += $"\nSelected Icon: {gui.selectedIcon}";
        debugText.text += $"\nHeld Item: {(InventoryGUI.heldObject ? InventoryGUI.heldObject.name : "<empty>")}";
    }

}
