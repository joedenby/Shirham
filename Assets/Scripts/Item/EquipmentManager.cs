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
        var slot = GetSlot(item.equipType);

        if (slot != null)  {
            previous = slot.item;
            slot.SetItem(item);
            return;
        }

        previous = null;
        Debug.LogError($"No slot found for item {item.name}");
    }

    public void RemoveItem(EquipType equipType)
    {
        GetSlot(equipType).SetItem(null);
    }

    public Equipment GetItem(EquipType equipType) {
        return GetSlot(equipType)?.item;
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

    public EquipSlot GetSlot(EquipType equipType) {
        return equipType switch
        {
            _ when equipType == EquipType.Head => head,
            _ when equipType == EquipType.Armor => armor,
            _ when equipType == EquipType.MainHand => mainHand,
            _ when equipType == EquipType.OffHand => offHand,
            _ when equipType == EquipType.Back => back,
            _ => null
        };
    }

    [ContextMenu("Assign Sprite Renderers")]
    public void AssignSpriteRenderers() {
        //Head child @ 2, 0, 4, 0
        Transform child = Util.GetChild(transform, 0, 0 ,0 , 2, 0, 4, 0);
        head.spriteRenderer = head.spriteRenderer ? head.spriteRenderer :
            child.GetComponent<SpriteRenderer>();

        //Armor child @ 1, 1, 0
        child = Util.GetChild(transform, 0, 0, 0, 1, 1, 0);
        armor.spriteRenderer = armor.spriteRenderer ? armor.spriteRenderer :
            child.GetComponent<SpriteRenderer>();

        //MainHand child @ 3, 1, 0, 1, 0
        child = Util.GetChild(transform, 0, 0, 0, 3, 1, 0, 1, 0);
        mainHand.spriteRenderer = mainHand.spriteRenderer ? mainHand.spriteRenderer : 
            child.GetComponent<SpriteRenderer>();

        //OffHand child @ 3, 0, 0, 2, 0
        child = Util.GetChild(transform, 0, 0, 0, 3, 0, 0, 2, 0);
        offHand.spriteRenderer = offHand.spriteRenderer ? offHand.spriteRenderer :
            child.GetComponent<SpriteRenderer>();

        //Back child @ 0, 0
        child = Util.GetChild(transform, 0, 0, 0, 0, 0);
        back.spriteRenderer = back.spriteRenderer ? back.spriteRenderer :
            child.GetComponent<SpriteRenderer>();
     
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
            if (item && item.equipType != equipType) { 
                Debug.LogError($"Item {item.name} is not {equipType} type!");
                return;
            }

            this.item = item;
            UpdateSprite();
        }

        public void UpdateSprite() {
            if (!spriteRenderer) return;
            spriteRenderer.sprite = item?.icon;
        }

        public void AssignMaterial(Material mat) {
            if (!spriteRenderer) return;
            spriteRenderer.material = mat;
        }

    }
}
