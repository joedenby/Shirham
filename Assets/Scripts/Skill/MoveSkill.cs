using GameManager.Battle;
using UnityEngine;


[System.Serializable, CreateAssetMenu(menuName = "MoveSkill")]
public class MoveSkill : ScriptableObject
{
    public Pattern pattern;
    public Instruction[] instructions;

    public void GetMovementSpace() {
        BattleEvent.SquareCallEvent += SetTargetSquare;
    }

    public void SetTargetSquare() {
        BattleGrid.UnitToSquare(BattleSystem.GetCurrentUnit(), BattleGrid.Selected, true);
        BattleGrid.SetGridState(BattleSquare.BattleSquareState.Closed);
        BattleEvent.SquareCallEvent -= SetTargetSquare;
        BattleGrid.ResetGrid();
    }
}
