using GameManager.Battle;
using GameManager.Hub;
using UnityEngine;

[System.Serializable, CreateAssetMenu(menuName = "Skill")]
public class Skill : ScriptableObject
{
    [SerializeField] private bool singleTarget;
    [TextArea(3, 5)] public string description;
    [Min(0)]public int cost = 1;
    public Pattern pattern;
    public Sprite icon;
    public string anim;
    public string unitAnim;
    public AudioClip sound;
    public Instruction[] instructions;


    //Verifies if each instruction is legal, if so then pass to
    //battlesystem for processing. 
    public virtual void Cast() {
        BattleSquare square = BattleGrid.GetCurrentSquare();
        WorldAnimations.PlaySkillAnimation(anim,  (singleTarget ?
            new BattleSquare[] { BattleGrid.Selected } :
            BattleGrid.GetSquares(square, pattern)));

        BattleSystem.GetCurrentUnit().AnimationTrigger(unitAnim);
        foreach (Instruction i in instructions) {
            i.Cast(square, pattern);
        }

        AudioManager.PlaySound(sound);
    }


    public virtual Instruction HeaderInstruction() => instructions[0];

    public virtual Instruction DamageInstruction() {
        foreach (Instruction instruction in instructions) { 
            if(instruction.action.evt == Instruction.ActionEvent.Damage) 
                return instruction;
        }

        return new Instruction();
    }

    protected Stats GetCasterStats() {
        if (instructions.Length == 0) {
            Debug.LogError("No caster or instructions found for skill");
            return new Stats();
        }
            
        return BattleSystem.GetCurrentUnit().combatant.FinalStats();
    }

}
