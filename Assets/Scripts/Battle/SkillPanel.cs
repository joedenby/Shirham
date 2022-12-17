using UnityEngine;
using GameManager.Battle;

public class SkillPanel : UIComponent
{
    [SerializeField] private SkillIcon[] skillIcons = new SkillIcon[4];



    public override void Enable() {
        if (!BattleSystem.GetCurrentUnit()) return;

        Combatant combatant = BattleSystem.GetCurrentUnit().combatant;
        for (int i = 0; i < skillIcons.Length; i++) {
            if (i < combatant.Skills.Count) {
                skillIcons[i].Set(combatant.Skills[i]);
                skillIcons[i].gameObject.SetActive(true);
            }
            else {
                skillIcons[i].gameObject.SetActive(false);
            }
        }

        gameObject.SetActive(true);
    }

    public override void Disable()  {
        foreach (SkillIcon icon in skillIcons)
            icon.gameObject.SetActive(false);

        gameObject.SetActive(false);
    }



}
