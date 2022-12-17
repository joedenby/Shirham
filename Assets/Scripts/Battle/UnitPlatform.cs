using UnityEngine;
using UnityEngine.UI;
using GameManager.Battle;

public class UnitPlatform : UIComponent
{
    
    private Vector2 startPositon;
    [SerializeField] private Image platformImage;
    [SerializeField] private GameObject buttons;
    [SerializeField] private float offset;
    


    private void Awake() => startPositon = transform.localPosition;

    private void FixedUpdate() => buttons.SetActive(!BattleSystem.GetCurrentUnit().isMoving);
  
    public override void Enable() {
        var sq = BattleGrid.GetSquareViaUnit(BattleSystem.GetCurrentUnit());
        gameObject.SetActive(true);
    } 

    public override void Disable() {
        gameObject.SetActive(false);
        transform.localPosition = startPositon;
    }

    public void Pop() {
        Enable();
        LeanTween.cancel(gameObject);
        transform.localPosition = startPositon;
        LeanTween.moveLocalY(gameObject, offset, 1f).setEaseOutElastic().overshoot = 0.5f;
    }


}
