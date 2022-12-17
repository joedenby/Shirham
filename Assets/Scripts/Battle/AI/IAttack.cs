using GameManager.Battle;
using GameManager;
using UnityEngine;

[System.Serializable]
public class IAttack : IModule
{
    EnemyUnit unit;
    Skill desiredSkill;
    UnitController desiredTarget;

    [SerializeField]private bool active;
    [SerializeField, Range(0f, 1f)]private float baseValue = 0.5f;

    //TODO: This is a bastic first draft that doesn't do anything
    //other than attack players with single target attacks. Expand on this later.
    public float Confidence()
    {
        if(baseValue == 0) return 0;

        BattleSquare center = BattleGrid.GetSquareViaUnit(unit);
        Skill[] unitSkills = unit.combatant.Skills.ToArray();
        float c = 0;

        foreach(Skill skill in unitSkills) {
            BattleSquare[] squares = BattleGrid.GetSquares(center, skill.pattern);

            foreach (BattleSquare sq in squares) {
                if (!sq.ContainsUnits()) continue;  //Is a unit on this square?

                UnitController targetUnit = sq.GetInhabitedUnits()[0]; 
                if (targetUnit.IsEnemy()) continue; //Is this unit a party member?

                Instruction instruction = skill.DamageInstruction();
                if (instruction.action.evt != Instruction.ActionEvent.Damage) 
                    continue;   //Does this skill damage the unit?

                //Valid target, do the math.
                Elemental[] damageAsElementalTable = Parse.ParseDamage(instruction.action.value, center, sq);
                float damage = Elemental.TotalValue(damageAsElementalTable);
                float hp = (float)(damage / targetUnit.combatant.MaxHP());
                float a = unit.combatant.GetAggroRelative(targetUnit.combatant, BattleGrid.Distance(center, sq));

                float confidence = baseValue + (hp + (a * 0.1f));
                if (confidence < c) continue; //Skill on target less confident. Skip.

                //Most confident so far. Assign as desired.
                c = confidence;
                desiredTarget = targetUnit;
                desiredSkill = skill;
            }
        }

        return c;
    }

    public bool IsActive() => active;

    public void PerformAction()
    {
        if (desiredSkill == null || desiredTarget == null) {
            Debug.LogError("Tried to perform attack instruction with missing parameters.");
            return;
        }

        desiredSkill.Cast();
    }

    public void SetEnemyUnit(EnemyUnit unit) => this.unit = unit;
}
