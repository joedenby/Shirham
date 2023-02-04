using GameManager.Battle;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class SkillIcon : UIComponent
{
    [SerializeField] private Skill skill;
    [SerializeField] private TextMeshProUGUI header;
    [SerializeField] private Image icon;


    public void Set(Skill skill) { 
        this.skill = skill;
        icon.sprite = skill.icon;
        header.text = skill.name;
    }
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
        BattleGrid.awaitingSquareSelection = true;
        BattleUI.instance.Disable();
    }

    protected override void MouseEnter() => ShowPattern(skill.HeaderInstruction().singleTarget);

    protected override void MouseExit() => ClearPattern();
    public void ShowPattern(bool validOnly) {
        ClearPattern();

        BattleSquare[] squares = BattleGrid.GetSquares(BattleGrid.GetSquareViaUnit(
            BattleSystem.GetCurrentUnit()), skill.pattern, validOnly ? 
                skill.pattern.states : BattleSquare.AvailableSquareStates);

        BattleGrid.SetGridState(BattleSquare.BattleSquareState.Selectable, squares);
    }


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
