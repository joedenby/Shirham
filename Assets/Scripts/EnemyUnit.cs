using GameManager.Battle;
using GameManager.Units;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyUnit : UnitController
{
    [Min(0)] 
    public float sightRadius = 3f;
    public float alertRadius = 6f;
    private bool inBattle = false;
    private bool inRange => Vector2.Distance(UnitManager.Player.transform.position, transform.position) < (sightRadius * 1.5f);
    private Vector2 point;


    private void Awake() => SyncPlayerCheck();

    private void OnDestroy() => UnitManager.EnemyUnits.Remove(this);

    private void OnEnable() {
        if (UnitManager.EnemyUnits.Contains(this)) return;
        UnitManager.EnemyUnits.Add(this);

        gameObject.layer = 2;
    }

    private void FixedUpdate()
    {
        if (!gameObject.activeSelf) return;

        Animate();
        Pathing();
    }

    async void SyncPlayerCheck() {
        await Task.Delay(200);
        if (!Application.isPlaying || inBattle) return;

        inBattle = CheckBattle();
        SyncPlayerCheck();
    }

    private bool CheckBattle() {
        if (inBattle) return true;
        if (!UnitManager.Player) return false;


        bool battleInProgress = BattleSystem.State == BattleState.Active;
        if (!battleInProgress && !HasPlayerSight()) 
            return false;

        Collider2D[] hit2Ds = Physics2D.OverlapCircleAll(transform.position, battleInProgress ? sightRadius : alertRadius);
        foreach (Collider2D hit in hit2Ds) {
            var unit = hit.GetComponent<UnitController>();
            var sq = hit.GetComponent<BattleSquare>();

            if (unit && unit.IsPlayer())
            {
                BattleSystem.GoBattle(this);
                inBattle = true;
                return true;
            }
            else if (sq) {
                BattleSystem.GoBattle(this);
                inBattle = true;
                return true;
            } 
        }

        return false;
    }

    private bool HasPlayerSight() {
        Vector2 dir = -(transform.position - UnitManager.Player.transform.position);
        dir.y += 0.2f;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, sightRadius);
        point = hit.point;
        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || !inRange)
            return;
        // Draws a 5 unit long red line in front of the object
        Gizmos.color = HasPlayerSight() ? Color.red : Color.white;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
        Gizmos.DrawSphere(point, 0.1f);
        Gizmos.DrawLine((Vector2)transform.position, (Vector2)UnitManager.Player.transform.position);
    }
}
