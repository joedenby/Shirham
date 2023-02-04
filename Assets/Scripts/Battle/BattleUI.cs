using GameManager.Battle;
using UnityEngine;

public class BattleUI : UIComponent
{
    public static BattleUI instance { get; private set; }
    public static BattleUIMenu menu { get; private set; }
    public enum BattleUIMenu { 
        Home, Move, Attack, Item
    }
    private InputMaster.BattleActions battleInput;
    [SerializeField] private UIComponent nextButton;
    [SerializeField] private GameObject actionButtonsParent;
    [SerializeField] private SkillPanel skillPanel;


    private void Awake()  {
        instance = this;
        battleInput = GameManager.InputManager.BattleInput();
        battleInput.Back.performed += ev => GoBack();
        Disable();
    }

    public void SetMenu(BattleUIMenu panelMenu) {
        menu = panelMenu;
        skillPanel.Disable();

        switch (menu) {
            case BattleUIMenu.Move:
                var moveSquares = BattleGrid.GetMoveSquares(BattleSystem.GetCurrentUnit());
                BattleGrid.SetGridState(BattleSquare.BattleSquareState.Selectable, moveSquares);
                break;
            case BattleUIMenu.Attack:
                skillPanel.Enable();
                break;
            default:
                if (BattleGrid.awaitingSquareSelection) {
                    BattleGrid.awaitingSquareSelection = false;
                }
                BattleGrid.ResetGrid();
                nextButton.Enable();
                actionButtonsParent.SetActive(true);
                return;
        }

        BattleEvent.SquareCallEvent += GoBack;
        actionButtonsParent.SetActive(false);
    }

    private void GoBack() {
        if (menu == BattleUIMenu.Home) return;
        SetMenu(BattleUIMenu.Home);
        Enable();
        BattleGrid.awaitingSquareSelection = false;
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
        BattleGrid.awaitingSquareSelection = true;
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
