using UnityEngine;

[CreateAssetMenu(menuName = "Item/Equipment")]
public class Equipment : Item
{
    public EquipType equipType;
    public Stats stats;
    public Elemental[] enhancments;
    public Elemental[] resistances;
}
