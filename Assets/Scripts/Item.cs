using UnityEngine;
using UnityEngine.UI;

[System.Serializable, CreateAssetMenu(menuName = "Item/Item")]
public class Item : ScriptableObject
{
    [Header("General")]
    public Sprite icon;
    public new string name;
    [TextArea(3, 5)]
    public string description;
    public float weight;
    public int price;
}
