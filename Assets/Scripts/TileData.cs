using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Environment/TileData")]
public class TileData : ScriptableObject
{
    public TileBase[] tiles;
    public ElementalType elementalType;
}
