using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class InventoryGUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject window;
    [SerializeField] private Transform slots;


    public void ShowInventory(bool active) {
        window.SetActive(active);
    }
}
