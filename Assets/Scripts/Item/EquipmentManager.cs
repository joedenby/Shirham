using System.Linq;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [Header("Configuration")]
    public bool canEquip = false;

    [Header("Slots")]
    public EquipSlot head = new EquipSlot(EquipType.Head);
    public EquipSlot armor = new EquipSlot(EquipType.Armor);
    public EquipSlot mainHand = new EquipSlot(EquipType.MainHand);
    public EquipSlot offHand = new EquipSlot(EquipType.OffHand);
    public EquipSlot back = new EquipSlot(EquipType.Back);


    private void OnValidate()
    {
        head.UpdateSprite();
        armor.UpdateSprite();
        mainHand.UpdateSprite();
        offHand.UpdateSprite();
        back.UpdateSprite();
    }

    public void EquipItem(Equipment item, out Equipment previous) {
        var slot = item.equipType switch  {
            _ when item.equipType == EquipType.Head => head,
            _ when item.equipType == EquipType.Armor => armor,
            _ when item.equipType == EquipType.MainHand => mainHand,
            _ when item.equipType == EquipType.OffHand => offHand,
            _ when item.equipType == EquipType.Back => back,
            _ => null
        };

        if (slot != null)
        {
            previous = slot.item;
            slot.SetItem(item);
            return;
        }

        previous = null;
        Debug.LogError($"No slot found for item {item.name}");
    }

    public Stats GearStats() { 
        return head.stats + armor.stats +  
            mainHand.stats + offHand.stats + back.stats;
    }

    public Elemental[] GearEnhancments() { 
        //I hate this solution. Please improve later >:(
        var result = Elemental.NewElementalTable();
        result = Elemental.Sum(result, head.item.enhancments);
        result = Elemental.Sum(result, armor.item.enhancments);
        result = Elemental.Sum(result, mainHand.item.enhancments);
        result = Elemental.Sum(result, offHand.item.enhancments);
        return Elemental.Sum(result, back.item.enhancments);
    }

    public Elemental[] GearResistances()
    {
        //I hate this solution. Please improve later >:(
        var result = Elemental.NewElementalTable();
        result = Elemental.Sum(result, head.item.resistances);
        result = Elemental.Sum(result, armor.item.resistances);
        result = Elemental.Sum(result, mainHand.item.resistances);
        result = Elemental.Sum(result, offHand.item.resistances);
        return Elemental.Sum(result, back.item.resistances);
    }

    [System.Serializable]
    public class EquipSlot
    {
        public Equipment item;
        public SpriteRenderer spriteRenderer;
        private EquipType equipType;
        public Stats stats => item ? item.stats : new Stats();

        public EquipSlot(EquipType equipType) {
            this.equipType = equipType;
        }

        public void SetItem(Equipment item) {
            if (item.equipType != equipType) { 
                Debug.LogError($"Item {item.name} is not a {equipType}!");
                return;
            }

            this.item = item;
            UpdateSprite();
        }

        public void UpdateSprite() {
            if (!spriteRenderer) return;

            spriteRenderer.sprite = item?.icon;
        }
    }
}
