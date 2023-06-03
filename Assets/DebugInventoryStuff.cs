using UnityEngine;
using TMPro;
using System.Drawing.Text;
using System;
using UnityEditor;
using UnityEngine.UI;

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
        debugText.text += $"\nHeld Item: {(Inventory.heldObject ? Inventory.heldObject.name : "<empty>")}";
    }

    [ContextMenu("Add Canvas")]
    private void AddCanvas() {
        Debug.Log("Started add...");
        var prefabs = Resources.LoadAll<GameObject>("Units");
        var unitCanvas = Resources.Load<GameObject>("Misc/UnitCanvas");

        foreach(var unit in prefabs)
        {
            Debug.Log($"Found: {unit.name}");

            bool hasCanvas = false;
            foreach (Transform child in unit.transform) {
                if (child.name.Equals("UnitCanvas")) { 
                    hasCanvas = true; 
                    break;
                }
            }

            
            GameObject unitPrefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(unit);
            if (!unitPrefabInstance.GetComponent<Inventory>())
            {
                Debug.Log($" >> Added Inventory to {unit.name}");
                unitPrefabInstance.AddComponent<Inventory>();
            }
            else {
                Debug.Log($" >> {unit.name} already has Inventory");
            }

            if (hasCanvas)
            {
                Debug.Log($" >> {unit.name} already has UnitCanvas");
            }
            else {
                Debug.Log($" >> Added UnitCanvas to {unit.name}");
                GameObject unitCanvasPrefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(unitCanvas);
                var window = unitCanvasPrefabInstance.transform.GetChild(0).GetChild(1);
                foreach(Transform icon in window)  {
                    icon.gameObject.GetComponent<ItemIcon>().inventory = unitPrefabInstance.GetComponent<Inventory>();
                }

                unitCanvasPrefabInstance.transform.SetParent(unitPrefabInstance.transform, false);
            }


            PrefabUtility.SaveAsPrefabAsset(unitPrefabInstance, AssetDatabase.GetAssetPath(unit));
            DestroyImmediate(unitPrefabInstance);
        }
    }
}
