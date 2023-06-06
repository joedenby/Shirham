using System;
using UnityEngine;

public class EquipmentGUI : MonoBehaviour
{
    public EquipmentManager equipment;

    [Header("Materials")]
    [SerializeField] private Material spriteMat;
    [SerializeField] private Material outlineMat;


    private void Start()
    {
        equipment = transform.parent.GetComponentInParent<EquipmentManager>();
    }

    public void PopOff(string placement) {
        if (Enum.TryParse(placement, true, out EquipType equipType)) {
            Equipment item = equipment.GetItem(equipType);
            if (!item) return;

            
            ItemObject.Instantiate(item, transform.position, true);
            equipment.RemoveItem(equipType);
            return;
        }

        Debug.LogError($"Specified placement not recognized.");
    }

    public void Highlight(string placement) {
        if (Enum.TryParse(placement, true, out EquipType equipType))
        {
            var slot = equipment.GetSlot(equipType);
            if (slot is null) return;
            if (slot.item is null) return;

            Material mat = new Material(outlineMat);
            mat.SetFloat("_OutlineThickness", 0.5f);
            mat.SetColor("_OutlineColor", Item.RarityAsColor(slot.item.rarity));
            slot.AssignMaterial(mat);
            
            return;
        }

        Debug.LogError($"Specified placement not recognized.");
    }

    public void RemoveHighlight(string placement) {
        if (Enum.TryParse(placement, true, out EquipType equipType))
        {
            
            var slot = equipment.GetSlot(equipType);
            if (slot is null) return;
            Material mat = new Material(spriteMat);
            slot.AssignMaterial(mat);

            return;
        }

        Debug.LogError($"Specified placement not recognized.");
    }

}
