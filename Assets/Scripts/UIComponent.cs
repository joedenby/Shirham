using UnityEngine;
using UnityEngine.EventSystems;

public class UIComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GUIAnchor GUIAnchor;

    public virtual void Enable() { 
        gameObject.SetActive(true);
    }

    public virtual void Disable() {
        gameObject.SetActive(false);
    }

    protected virtual void MouseEnter() {
    }

    protected virtual void MouseExit() {

    }

    protected void SetPosition(Vector2 position) => transform.position = position;

    protected void SetAnchorPosition() => SetPosition(GUIAnchor.WorldPosition());

    public void OnPointerEnter(PointerEventData eventData) {
        MouseEnter();
    }

    public void OnPointerExit(PointerEventData eventData) {
        MouseExit();
    }

}
