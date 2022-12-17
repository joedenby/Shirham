using System.Collections.Generic;
using UnityEngine;

/*===============================================
* ================== COMBATANT ==================
* ===============================================
*  Representation of combatant data. This 
*  includes stats used in combat move calculations.
*/
[System.Serializable]
public class Combatant
{
    public MoveSkill Movement;
    public List<Skill> Skills = new List<Skill>();
    public Stats BaseStats;
    public Stats GearStats;
    public Elemental[] enhancement = Elemental.NewElementalTable();
    public Elemental[] resitance = Elemental.NewElementalTable();
    private PreviousPerpetrator PP = new PreviousPerpetrator(null, 0);

    public bool isDead => (HP <= 0);
    public int HP;
    public int MP;
    public float AGGRO;


    public int MaxHP() => Mathf.RoundToInt(10 + (FinalStats().FinalEnd() * FinalStats().EndAsPercent()));
    public int MAXMP() => Mathf.RoundToInt(10 - (9 * FinalStats().EndAsPercent()));
    public float PercentHP() => HP > 0 ? ((float)HP / MaxHP()) : 0;
    public Stats FinalStats() => BaseStats.Combined(GearStats);
    public int TakeDamage(Combatant perpetrator, Elemental[] elementals)
    {
        Elemental[] final = Elemental.Neg(elementals, resitance);

        int sumTotal = Elemental.TotalValue(final);
        HP = (HP - sumTotal) > 0 ? (HP - sumTotal) : 0;
        PP = new PreviousPerpetrator(perpetrator, sumTotal);
        return sumTotal;
    }

    public float Vulnerability(Combatant aggressor) {
        var diff = Elemental.Neg(enhancement, aggressor.resitance);
        diff = Elemental.Purge(diff);
        if (diff.Length == 0) return 0;

        return Elemental.TotalValue(diff);
    }

    public float GetAggroRelative(Combatant aggressor, int distance) {
        float dd = PP.Perpetrator(aggressor) ? PP.DamageDelt : 0;
        float v = Vulnerability(aggressor);
        float d = (distance - 1 < 1) ? 1 : (1f / (distance - 1));

        return (dd != 0 ? (dd / MaxHP()) : 0) + //dd
            (v != 0 ? (v / 100) : 0) + //v
            d + AGGRO; // d + a
    }

    private struct PreviousPerpetrator {
        Combatant perpetrator;
        float damageDelt;

        public PreviousPerpetrator(Combatant perpetrator, float damageDelt) { 
            this.perpetrator = perpetrator;
            this.damageDelt = damageDelt;
        }

        public bool Perpetrator(Combatant suspect) => suspect.Equals(perpetrator);

        public float DamageDelt => damageDelt;
    }
}
