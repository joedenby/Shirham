using GameManager;
using GameManager.Units;
using GameManager.Hub;
using GameManager.Battle;
using System.Collections;
using UnityEngine;

public class TestSceneScript : MonoBehaviour
{
    public GameObject map;
    public UnitInstance player;
    public EnemyUnit enemyUnit;
    public Vector2 squarePos;
    public bool showTileInfo;
    public bool addParty;


    private IEnumerator Start()
    {
        Application.targetFrameRate = 165;
        Map.SetMap(map);

        var obj =  UnitManager.Instantiate(player);
        Runtime.SetPlayer(obj.gameObject);
        InputManager.PlayerInput().Move.Enable();

        if (addParty) {
            UnitController[] party = new UnitController[] {
            UnitManager.Instantiate(new UnitInstance("Unit003", Vector2.right, false))
            };
            yield return new WaitForEndOfFrame();

            foreach (UnitController unit in party)
                Runtime.AssignPartyMemeber(unit);
        }

        AudioManager.PlayMusic("HumanVillage", true, false);
        yield return null;
    }

    private void FixedUpdate()
    {
        if (!showTileInfo) return;
        Vector3 loc = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        loc.z = 0;

        Debug.Log($"Element @ tile {loc} = {Map.GetElementAtLocation(Vector3Int.FloorToInt(loc))}");
    }


    [ContextMenu("GetAggro")]
    private void GetAggro() {
        if (BattleSystem.State == BattleState.InActive) return;

        UnitController current = BattleSystem.GetCurrentUnit();
        float v = enemyUnit.combatant.Vulnerability(current.combatant);
        int dist = BattleGrid.Distance(BattleGrid.GetSquareViaUnit(enemyUnit), BattleGrid.GetSquareViaUnit(current));
        float a = enemyUnit.combatant.GetAggroRelative(current.combatant, dist);
        Debug.LogWarning($"Current: {current} \nEnemyUnit: {enemyUnit} \nv: {v} \na: {a} \ndist: {dist - 1}");
    }

    [ContextMenu("GotToSquare")]
    private void GoToSquare() {
        BattleGrid.UnitToSquare(enemyUnit, BattleGrid.GetSquareAtCoordinate(squarePos));
    }


}
