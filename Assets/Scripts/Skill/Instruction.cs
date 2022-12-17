using UnityEngine;
using GameManager.Battle;

[System.Serializable]
public struct Instruction
{
    public bool singleTarget;
    public bool friendly;
    public Action action;

    public Instruction(bool singleTarget, bool friendly, Action action) {
        this.action = action;
        this.friendly = friendly;
        this.singleTarget = singleTarget;
    }

    public void Cast(BattleSquare centre, Pattern pattern) {
        //Get target squares
        BattleSquare[] targetSquares = singleTarget ?
            new BattleSquare[] { BattleGrid.Selected } : 
            BattleGrid.GetSquares(centre, pattern);

        //Pass squares to battle invoke
        BattleSystem.InvokeInstruction(centre, targetSquares, this);
    }


    [System.Serializable]
    public struct Action {
        public ActionEvent evt;
        public string value;

        public Action(ActionEvent evt, string value) {
            this.evt = evt;
            this.value = value; 
        }

        public override string ToString() {
            return $"{evt} [{value}]";
        }
    }

    public enum ActionEvent { 
        Wait, KnockBack,  Damage, Stun, Spawn, Dialogue
    }

}