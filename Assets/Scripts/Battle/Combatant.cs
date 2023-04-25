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
    public bool isDead => (HP <= 0);
    public int HP = 1;
    public int MP = 1;
    public int MaxHP => Mathf.RoundToInt(10 + (combatantStats.RelativeEnd * combatantStats.EndAsPercent));
    public int MAXMP => Mathf.RoundToInt(10 - (9 * combatantStats.EndAsPercent));
    public float AGGRO;

    [Header("Configuration")]
    public MoveSkill movement;
    public List<Skill> skills = new List<Skill>();
    public EquipmentManager equipment;

    [Header("Stats/Elementals")]
    public Stats baseStats = new Stats(1, 0, 1);
    public Elemental[] baseEnhancement = Elemental.NewElementalTable();
    public Elemental[] baseResitance = Elemental.NewElementalTable();

    public Stats combatantStats => (equipment ? equipment.GearStats() : new Stats()) + baseStats;
    public Elemental[] combatantEnhancements => Elemental.Sum((equipment ? equipment.GearEnhancments() : Elemental.NewElementalTable()), baseEnhancement);
    public Elemental[] combatantResistances => Elemental.Sum((equipment ? equipment.GearResistances() : Elemental.NewElementalTable()), baseResitance);

    private PreviousPerpetrator PP = new PreviousPerpetrator(null, 0);

   
    public float PercentHP() => HP > 0 ? ((float)HP / MaxHP) : 0;
   
    public int TakeDamage(Combatant perpetrator, Elemental[] elementals)
    {
        Elemental[] final = Elemental.Neg(elementals, baseResitance);

        int sumTotal = Elemental.TotalValue(final);
        HP = (HP - sumTotal) > 0 ? (HP - sumTotal) : 0;
        PP = new PreviousPerpetrator(perpetrator, sumTotal);
        return sumTotal;
    }
    
    public float Vulnerability(Combatant aggressor) {
        var diff = Elemental.Neg(baseEnhancement, aggressor.baseResitance);
        diff = Elemental.Purge(diff);
        if (diff.Length == 0) return 0;

        return Elemental.TotalValue(diff);
    }
    
    public float GetAggroRelative(Combatant aggressor, int distance) {
        float dd = PP.Perpetrator(aggressor) ? PP.DamageDelt : 0;
        float v = Vulnerability(aggressor);
        float d = (distance - 1 < 1) ? 1 : (1f / (distance - 1));

        return (dd != 0 ? (dd / MaxHP) : 0) + //dd
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
