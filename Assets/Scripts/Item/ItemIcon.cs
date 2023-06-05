using UnityEngine;
using UnityEngine.UI;

/**
 * ==================================== ItemIcon.cs =============================================
 * The ItemIcon class is responsible for managing the display and interaction of individual item icons 
 * in an inventory UI. It allows assigning items to the icons, updating their appearance, and handling 
 * drag and drop operations. The class includes methods for setting the selected state of an item with 
 * color feedback, and for updating the icon's appearance based on the assigned item. Additionally, it 
 * can enable or disable dragging for the item icon, updating the UI as necessary.
 */
public class ItemIcon : MonoBehaviour
{
    [SerializeField] private Item item;
    [SerializeField] private Sprite defaultIcon;
    public Inventory inventory;

    private Image iconImage => GetComponent<Image>();
    private Image itemImage => transform.GetChild(0).GetComponent<Image>();
    // Property to track whether the item is being dragged or not.
    public bool isBeingDragged { get; private set; }
    private Vector2 mousePosition => CameraController.main.Camera.ScreenToWorldPoint(Input.mousePosition);
    private Vector2 returnPosition;
    // Get the index of this item icon in the parent transform hierarchy.
    private int index => transform.GetSiblingIndex();


    // Subscribe to the inventory update event in the Start method.
    private void Start() =>
        inventory.onInventoryUpdate.AddListener(UpdateIcon);


    // Assign a new item to the icon.
    public void AssignItem(Item newItem) {
        item = newItem;
        UpdateIcon();
    }

    // Update the icon when dragging is active.
    private void Update()
    {
        if (!isBeingDragged || !Inventory.heldObject) return;
        if (Inventory.heldObject.beingDragged == false) {
            SetDragged(false);
            return;
        }

        Inventory.heldObject.transform.position = mousePosition;
    }

    // Update the icon when changes are made in the editor.
    private void OnValidate() => UpdateIcon();

    // Update the icon's appearance based on the assigned item.
    public void UpdateIcon()
    {
        name = $"[Icon] {(item ? item.name : "<empty>")}";
        itemImage.sprite = item ? item.icon : defaultIcon;
    }

    // Set the selection state for the item icon, with color feedback.
    public void SetSelected(ItemObject held, bool isSelected)
    {
        LeanTween.cancel(gameObject);

        Color selectionColor = isSelected ? (held && item ? Color.red : Color.green) : Color.gray;

        // Using LeanTween.value() to modify the Image component's color directly
        LeanTween.value(gameObject, iconImage.color, selectionColor, 0.5f).setOnUpdate((Color colorValue) =>
        {
            iconImage.color = colorValue;
        });
    }

    // Set the dragging state for the item icon.
    public void SetDragged(bool active)
    {
        if (!item) return;

        if (active)
        {
            returnPosition = transform.position;
            
            Inventory.heldObject = ItemObject.Instantiate(item, mousePosition);
            Inventory.heldObject.SetToTopLayer(true);
            Inventory.heldObject.beingDragged = true;

            inventory.RemoveItem(index);
        }
        else
        {
            if (!InventoryGUI.activeWindow && GameManager.Hub.Navigation.ItemSafeLocation(mousePosition))
            {
                Inventory.heldObject.SetPosition(mousePosition);
                Inventory.heldObject.SetToTopLayer(false);
                inventory.RemoveItem(index);
            }

            transform.position = returnPosition;
        }

/*        iconImage.enabled = !active;
        itemImage.enabled = !active;*/
        isBeingDragged = active;
    }

}