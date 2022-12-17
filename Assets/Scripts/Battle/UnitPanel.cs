using GameManager.Battle;
using UnityEngine.UI;
using UnityEngine;

public class UnitPanel : UIComponent
{
    [SerializeField] private RawImage unitImage;


    public override void Enable() {
        MoveUnitUp();
        unitImage.CrossFadeAlpha(1, 0.2f, false);
    }

    public override void Disable() {
        unitImage.CrossFadeAlpha(0, 0.2f, false);
    }

    public void MoveUnitLeft(bool animate) {
        LeanTween.cancel(unitImage.gameObject);
        if (animate) LeanTween.moveLocalX(unitImage.gameObject, -640, 0.5f).setEaseLinear();
        else unitImage.transform.localPosition = new Vector3(-640, 0, 0);
    }

    public void MoveUnitCenter(bool animate) {
        LeanTween.cancel(unitImage.gameObject);
        if (animate) LeanTween.moveLocalX(unitImage.gameObject, 0, 0.5f).setEaseLinear();
        else unitImage.transform.localPosition = Vector3.zero;
    }

    public void MoveUnitUp() {
        LeanTween.cancel(unitImage.gameObject);
        unitImage.transform.localPosition = new Vector3(0, -64, 0);
        LeanTween.moveLocalY(unitImage.gameObject, 0, 0.2f).setEaseLinear();
    }
}
