using UnityEngine;

[System.Serializable]
public class Item : ScriptableObject
{
    [Header("General")]
    public Sprite icon;
    public new string name;
    [TextArea(3, 5)]
    public string description;
    public float weight;
    public int price;
    public Rarity rarity = Rarity.Common;
    public enum Rarity { 
        Common, Rare, Epic, Legendary
    }


    public virtual void Use() {
        Debug.Log($"Using {name}");
    }

    public static Color RarityAsColor(Rarity rarity) { 
        switch (rarity)
        {
            default: return Color.white;
                case Rarity.Epic: return Color.magenta;
                case Rarity.Rare: return Color.blue;
                case Rarity.Legendary: return new Color(1f, 0.5f, 0);
        }
    }

}
