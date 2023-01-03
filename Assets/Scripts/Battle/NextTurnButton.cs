using GameManager.Battle;

public class NextTurnButton : UIComponent { 
    public void OnClick() => BattleSystem.NextTurn(); 
}
