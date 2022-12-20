using GameManager.Battle;

public class BattleUI : UIComponent
{
    public static BattleUI main { get; private set; }
    public static BattleUIMenu menu { get; private set; }
    public enum BattleUIMenu { 
        Home, Move, Attack, Item
    }
    private InputMaster.BattleActions battleInput;


    private void Awake()  {
        main = this;
        battleInput = GameManager.InputManager.BattleInput();
        battleInput.Back.performed += ev => GoBack();
        Disable();
    }

    public void SetMenu(BattleUIMenu panelMenu) {
        menu = panelMenu;

        switch (menu) {
            case BattleUIMenu.Move:
                BattleEvent.SquareCallEvent += GoBack;
                BattleGrid.GetMoveSquares(BattleSystem.GetCurrentUnit());
                break;
            case BattleUIMenu.Attack:

                return;
            default:
                if (BattleGrid.awaitingSquareSelection) {
                    BattleGrid.awaitingSquareSelection = false;
                }
                BattleGrid.ResetGrid();

                return;
        }

    }

    private void GoBack() {
        if (menu == BattleUIMenu.Home) return;
        SetMenu(BattleUIMenu.Home);
    }

    public override void Enable()
    {
        battleInput.Enable();
        SetMenu(BattleUIMenu.Home);
        BattleEvent.PartyMemberChangeEvent += GoBack;
        SetAnchorPosition();
        gameObject.SetActive(true);
    }

    public override void Disable() {
        battleInput.Disable();
        BattleEvent.PartyMemberChangeEvent -= GoBack;
        gameObject.SetActive(false);
    }

    public void SelectMove() {
        var current = BattleSystem.GetCurrentUnit().combatant;
        if (current == null) return;

        SetMenu(BattleUIMenu.Move);
        current.Movement.GetMovementSpace();
    }

    public void SelectAttack() {
        SetMenu(BattleUIMenu.Attack);
    }

    public void ShowMove(bool active) {
        if (BattleGrid.awaitingSquareSelection) return;
        if (!active) {
            BattleGrid.ResetGrid();
            return;
        }

        var combatant = BattleSystem.GetCurrentUnit().combatant;
        BattleSquare[] squares = BattleGrid.GetMoveSquares(BattleSystem.GetCurrentUnit());
        BattleGrid.SetGridState(BattleSquare.BattleSquareState.Selectable, squares);
    }

    public void ShowAttack(bool active) {
        if (!active) {
            return;
        }
    }

    public void ShowItem(bool active) {
        if (!active) {
            return;
        }
    }

}
