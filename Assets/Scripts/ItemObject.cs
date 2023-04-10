using UnityEngine;

public class ItemObject : Interactable
{
    public Item item;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    private Vector3 _offset;
    private float _zCoordinate;
    private Camera _mainCamera;


    private void Start()
    {
        actionIcon = item ? item.icon : null;
        promptText = $"Pick up {(item ? item.name : "???")}";
        spriteRenderer.sprite = item ? item.icon : null;
        _mainCamera = Camera.main;
    }

    private void OnMouseDown()
    {
        _zCoordinate = _mainCamera.WorldToScreenPoint(gameObject.transform.position).z;
        _offset = gameObject.transform.position - GetMouseWorldPosition();
        showPrompt = false;
        SetToTopLayer(true);
    }

    private void OnMouseDrag()
    {
        transform.position = new Vector3(GetMouseWorldPosition().x + _offset.x, GetMouseWorldPosition().y + _offset.y, transform.position.z);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = _zCoordinate;
        return _mainCamera.ScreenToWorldPoint(mousePoint);
    }

    private void OnMouseUp()
    {
        showPrompt = true;
        transform.position = GameManager.Hub.Navigation.CenterSquare(transform.position);
        SetToTopLayer(false);
    }

    private void SetToTopLayer(bool active)
    {
        spriteRenderer.sortingLayerName = active ? "Highest" : "Medium";
        spriteRenderer.sortingOrder = active ? 11 : 0;
        spriteRenderer.color = new Color(1, 1, 1, active ? 0.5f : 1);
    }
}
