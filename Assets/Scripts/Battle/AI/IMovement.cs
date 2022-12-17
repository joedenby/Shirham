using UnityEngine;
using GameManager.Battle;
using System.Linq;

[System.Serializable]
public class IMovement : IModule
{
    private EnemyUnit unit;
    private BattleSquare desiredSquare;

    [SerializeField] private bool active;
    [SerializeField, Range(0, 1)] public float baseValue = 0.5f;


    public void SetEnemyUnit(EnemyUnit unit) => this.unit = unit;

    public float Confidence() {
        //Get square EnemyUnit is standing on
        BattleSquare center = BattleGrid.GetSquareViaUnit(unit);

        //Get all possible movement squares
        BattleSquare[] squares = BattleGrid.GetMoveSquares(unit, unit.combatant.Movement.pattern);

        desiredSquare = null;   
        float confidence = 0;
        int p2 = 0; //Party members surrounding EnemyUnit


        //Party members in radial area of square EnemyUnit is currenly standing on
        BattleSquare[] centerNeighbours = BattleGrid.Radial(center);
        foreach (BattleSquare cn in centerNeighbours) {
            if (!cn.ContainsUnits()) continue;

            UnitController u = cn.GetInhabitedUnits()[0];
            if (!u.IsEnemy()) {
                p2++;
            }
        }

        foreach (BattleSquare sq in squares) {
            int dist = BattleGrid.Distance(center, sq);
            if (dist > unit.combatant.MP) continue;

            int p = 0;  //Party in radial area
            int e = 0;  //EnemyUnits in radial area
            float a = 0;//Combined aggro for party in radial area 
            
            BattleSquare[] neighbours = BattleGrid.Radial(sq);
            foreach (BattleSquare n in neighbours) {
                if (!n.ContainsUnits()) continue;

                UnitController u = n.GetInhabitedUnits()[0];
                if (u.IsEnemy()) {
                    e++;
                }
                else {
                    p++;
                    a += unit.combatant.GetAggroRelative(u.combatant, BattleGrid.Distance(center, n));
                }
            }


            e = (e > 0) ? (e - 1) : 0;
            float c = ((baseValue + (e * 0.1f)) - ((p2 * 0.1f) - (p * 0.1f))) + (a * 0.1f);
            if (c > confidence) desiredSquare = sq;

            confidence = (c > confidence) ? c : confidence;
        }

        Debug.Log($"Chosen {(desiredSquare != null ? desiredSquare.name : "NULL")} with c: {confidence}");
        return confidence; 
    }

    public bool IsActive() => active;

    public void SetActive(bool enabled) => active = enabled;

    public void PerformAction()
    {
        if(!desiredSquare) return;
        BattleGrid.UnitToSquare(unit, desiredSquare, true);
    }

    
}
