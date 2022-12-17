using TMPro;
using GameManager.Battle;
using UnityEngine;

public class BattleSquareDebug : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI debugTxt;

    void Update() {
        debugTxt.text = $"Selected: {(BattleGrid.Selected ? BattleGrid.Selected.name : "NONE")} | " +
            $"awaitSS: [{BattleGrid.awaitingSquareSelection}] | dist: {Dist()}";
        debugTxt.color = BattleGrid.awaitingSquareSelection ? Color.green : Color.yellow;
    }

    string Dist() {
        if (!BattleGrid.Selected) return "-";
        return BattleGrid.Distance(BattleGrid.GetSquareViaUnit(BattleSystem.GetCurrentUnit()), BattleGrid.Selected).ToString();
    }
}
