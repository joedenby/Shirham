using UnityEngine;
using GameManager.Hub;
using UnityEngine.EventSystems;
using System.Runtime.CompilerServices;

/**
 * ==================================== ItemObject.cs ====================================
 * The ItemObject class represents an interactable item in the game world, which can be picked up and 
 * added to the player's inventory. This class inherits from the Interactable base class and manages 
 * the item's display, interactions, and instantiation.
 */
public class ItemObject : Interactable
{
    public Item item;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private OutlineController outline;

    // Private fields for handling item's dragging and positioning
    private Vector3 _offset;
    private float _zCoordinate;
    public bool beingDragged;
    private Camera _mainCamera => CameraController.main.Camera;




    // Set pulse effect active when the object is created in the game world
    private void Start() => 
        SetPulse(true);

    // Cancel ongoing animations using LeanTween library when the object is disabled
    private void OnDisable() =>
        LeanTween.cancel(gameObject);


    // Update the item's name and sprite in the Unity Editor when the script is loaded or a value is changed
    private void OnValidate() {
        name = $"[Item] {(item ? item.name : "<empty>")}";
        spriteRenderer.sprite = item ? item.icon : null;
    }

    // Create and return a new ItemObject instance with the specified Item and position in the game world
    public static ItemObject Instantiate(Item item, Vector2 position = new Vector2(), bool startDragged = false)  {
        if (!item)
        {
            Debug.LogError("Tried to instantiate null item");
            return null;
        }

        var prefab = Resources.Load<GameObject>("Items/[Item]Obj");
        var instance = Instantiate(prefab, position, Quaternion.identity);
        var itemObject = instance.GetComponent<ItemObject>();
        itemObject.name = $"[Item] {(item ? item.name : "<empty>")}";
        itemObject.spriteRenderer.sprite = item ? item.icon : null;
        itemObject.item = item;
        itemObject.beingDragged = startDragged;

        if (startDragged)
            Inventory.heldObject = itemObject;

        return itemObject;
    }

    // Update the item's outline pulse effect
    private void UpdatePulse(float value) =>
        outline?.SetOutline(Item.RarityAsColor(item.rarity), value);

    // Activate or deactivate the item's pulse effect using LeanTween library
    private void SetPulse(bool active)  {

        if (!active)
        {
            LeanTween.cancel(gameObject);
            UpdatePulse(1);
            return;
        }

        LeanTween.value(gameObject, 0.75f, 1.5f, 1f)
            .setOnUpdate(UpdatePulse)
            .setEase(LeanTweenType.easeOutSine)
            .setLoopPingPong();
    }

    private void Update() {
        if (!beingDragged) return;
        if (!Input.GetKey(KeyCode.Mouse0)) {
            OnMouseUp();
            return;
        }

        transform.position = new Vector3(GetMouseWorldPosition().x + _offset.x, GetMouseWorldPosition().y + _offset.y, transform.position.z);
    }

    // Update private fields and item's layer when the mouse button is pressed down
    private void OnMouseDown()  {
        _zCoordinate = _mainCamera.WorldToScreenPoint(gameObject.transform.position).z;
        _offset = gameObject.transform.position - GetMouseWorldPosition();
        Inventory.onItemPickUp?.Invoke();
        Inventory.heldObject = this;
        SetToTopLayer(true);
        SetPulse(false);
        beingDragged = true;
    }

    // Convert mouse position from screen to world coordinates
    private Vector3 GetMouseWorldPosition()  {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = _zCoordinate;
        return _mainCamera.ScreenToWorldPoint(mousePoint);
    }

    // Snap the item's position to the grid, reset its layer, and activate pulse effect when the mouse button is released
    private void OnMouseUp()  {
        Debug.Log($"Up {name}");
        beingDragged = false;   
        SetToTopLayer(false);
        SetPulse(true);
        SetPosition(transform.position);
        Inventory.onItemDrop?.Invoke();

        if(!InventoryGUI.activeWindow)
            Inventory.heldObject = null;
    }

    public void SetPosition(Vector2 location) {
        LeanTween.cancel(gameObject);
        LeanTween.move(gameObject, GameManager.Hub.Navigation.CenterSquare(location), 0.33f).setEaseOutBounce();
    }

    // Set the item's sorting layer and order based on the "active" parameter.
    // Change the item's transparency when it's being dragged (active = true).
    public void SetToTopLayer(bool active)   {
        spriteRenderer.sortingLayerName = active ? "Highest" : "Medium";
        spriteRenderer.sortingOrder = active ? 11 : 0;
        spriteRenderer.color = new Color(1, 1, 1, active ? 0.5f : 1);
    }
}