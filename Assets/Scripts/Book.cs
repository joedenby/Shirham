using UnityEngine;

[System.Serializable, CreateAssetMenu(menuName = "Item/Book")]
public class Book : Item
{
    [Header("Book Information")]
    public string title;
    public bool firstEdition;
    [TextArea(10, 25)] public string text;
   
}
