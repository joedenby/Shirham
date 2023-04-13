using UnityEngine;

public class ItemObject : Interactable
{
    public Item item;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private OutlineController outline;

    private Vector3 _offset;
    private float _zCoordinate;
    private Camera _mainCamera;



    private void Start() {
        SetUp();
    }

    private void OnDisable() {
        LeanTween.cancel(gameObject);
    }

    [ContextMenu("SetUp")]
    private void SetUp() {
        if (!item) {
            Debug.LogWarning("No item has been assigned. ItemObject will be disabled");
            return;
        }

        name = $"[Item] {item.name}";
        spriteRenderer.sprite = item ? item.icon : null;
        _mainCamera = CameraController.main.Camera;
        SetPulse(true);
    }

    private void UpdatePulse(float value) =>
        outline?.SetOutline(Item.RarityAsColor(item.rarity), value);

    private void SetPulse(bool active) {
        if (!active) { 
            LeanTween.cancel(gameObject);
            UpdatePulse(1);
            return;
        }

        LeanTween.value(gameObject, 0.75f, 1.5f, 1f)
            .setOnUpdate(UpdatePulse)
            .setEase(LeanTweenType.easeOutSine)
            .setLoopPingPong();
    }

    private void OnMouseDown() {
        _zCoordinate = _mainCamera.WorldToScreenPoint(gameObject.transform.position).z;
        _offset = gameObject.transform.position - GetMouseWorldPosition();
        SetToTopLayer(true);
        SetPulse(false);
    }

    private void OnMouseDrag() {
        transform.position = new Vector3(GetMouseWorldPosition().x + _offset.x, GetMouseWorldPosition().y + _offset.y, transform.position.z);
    }

    private Vector3 GetMouseWorldPosition() {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = _zCoordinate;
        return _mainCamera.ScreenToWorldPoint(mousePoint);
    }

    private void OnMouseUp() {
        transform.position = GameManager.Hub.Navigation.CenterSquare(transform.position);
        SetToTopLayer(false);
        SetPulse(true);
    }

    private void SetToTopLayer(bool active)
    {
        spriteRenderer.sortingLayerName = active ? "Highest" : "Medium";
        spriteRenderer.sortingOrder = active ? 11 : 0;
        spriteRenderer.color = new Color(1, 1, 1, active ? 0.5f : 1);
    }
}
