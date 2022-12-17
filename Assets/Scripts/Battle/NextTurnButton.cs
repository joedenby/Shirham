using GameManager.Battle;
using UnityEngine;

public class NextTurnButton : MonoBehaviour{ 
    public void OnClick() => BattleSystem.NextTurn(); 
}
