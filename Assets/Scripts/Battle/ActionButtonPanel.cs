using UnityEngine.UI;
using UnityEngine;

public class ActionButtonPanel : UIComponent
{
    [SerializeField] private Button[] buttons;


    private void Start()
    {
        Disable();
    }

    public override void Enable() {
        foreach (Button btn in buttons) {
            btn.gameObject.SetActive(true);
        }
    }

    public override void Disable() {
        foreach (Button btn in buttons) {
            btn.gameObject.SetActive(false);
        }
    }

}
