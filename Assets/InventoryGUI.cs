using UnityEditor;
using UnityEngine.UI;
using UnityEngine;
using System.CodeDom;

[InitializeOnLoad]
public class InventoryGUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject bubble;
    [SerializeField] private GameObject window;


    private int selectedIcon = -1;



    public void ShowInventory(bool active) {
        window.SetActive(active);
        bubble.SetActive(!active);
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
    }
}
