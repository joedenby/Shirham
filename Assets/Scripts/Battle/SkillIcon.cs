using GameManager.Battle;
using UnityEngine.UI;
using UnityEngine;

public class SkillIcon : UIComponent
{
    [SerializeField] private Skill skill;
    [SerializeField] private Image icon;

   
    public void Set(Skill skill) => this.skill = skill;
    public override void Enable() => gameObject.SetActive(true);
    public override void Disable() => gameObject.SetActive(false);
    public void Select() {
        if (!skill.HeaderInstruction().singleTarget) {
            skill.Cast();
            return;
        }

        ShowPattern(true);
        BattleEvent.SquareCallEvent += skill.Cast;
        BattleEvent.SquareCallEvent += ClearEvents;
    }

    public void ShowPattern(bool validOnly) {
        ClearPattern();

        BattleSquare[] squares = BattleGrid.GetSquares(BattleGrid.GetSquareViaUnit(
            BattleSystem.GetCurrentUnit()), skill.pattern, validOnly ? 
                skill.pattern.states : BattleSquare.AvailableSquareStates);

        BattleGrid.SetGridState(BattleSquare.BattleSquareState.Selectable, squares);
    }


    //TODO: Fix this...
    // protected override void OnMouseStay() => TooltipUI.main?.Show(skill);

   // protected override void MouseExit() => TooltipUI.main?.Disable();

    public void ClearPattern() {
        if (BattleGrid.awaitingSquareSelection) return;
        BattleGrid.SetGridState(BattleSquare.BattleSquareState.Open);
    }

    private void ClearEvents() {
        ClearPattern();
        BattleEvent.SquareCallEvent -= skill.Cast;
        BattleEvent.SquareCallEvent -= ClearPattern;
    }

}
