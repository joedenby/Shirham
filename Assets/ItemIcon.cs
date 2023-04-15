using UnityEngine;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour
{
    [SerializeField] private Item item;
    [SerializeField] private Sprite defaultIcon;
    [SerializeField] private Inventory inventory;

    private Image iconImage => GetComponent<Image>();
    private Image itemImage => transform.GetChild(0).GetComponent<Image>();
    public bool isBeingDragged { get; private set; }
    private Vector2 mousePosition => CameraController.main.Camera.ScreenToWorldPoint(Input.mousePosition);
    private Vector2 returnPosition;
    private int index => transform.GetSiblingIndex();


    private void Start() =>
        inventory.onInventoryUpdate.AddListener(UpdateIcon);
   
    public void AssignItem(Item newItem) => 
        item = newItem;

    private void Update()
    {
        if (!isBeingDragged) return;
        InventoryGUI.heldObject.transform.position = mousePosition;
    }

    private void OnValidate() => 
        UpdateIcon();

    public void UpdateIcon() {

        name = $"[Icon] {(item ? item.name : "<empty>")}";
        itemImage.sprite = item ? item.icon : defaultIcon;
    }

    public void SetSelected(ItemObject held, bool isSelected) {
        LeanTween.cancel(gameObject);

        Color selectionColor = isSelected ? (held && item ? Color.red : Color.green) : Color.gray;

        // Using LeanTween.value() to modify the Image component's color directly
        LeanTween.value(gameObject, iconImage.color, selectionColor, 0.5f).setOnUpdate((Color colorValue) => {
            iconImage.color = colorValue;
        });
    }

    public void SetDragged(bool active) {
        if(!item) return;

        if (active) {
            returnPosition = transform.position;
      
            InventoryGUI.heldObject = ItemObject.Instantiate(item, mousePosition);
            InventoryGUI.heldObject.SetToTopLayer(true);
        }
        else {
            transform.position = returnPosition;
            InventoryGUI.heldObject.SetToTopLayer(false);
            InventoryGUI.heldObject = null;
            inventory.RemoveItem(index);
        }

        iconImage.enabled = !active;
        itemImage.enabled = !active;
        isBeingDragged = active;
    }

}
