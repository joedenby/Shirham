using UnityEngine;
using GameManager.Battle;
using System.Collections.Generic;

public class BattleSquare : MonoBehaviour
{
    [SerializeField] private float hpOffset;
    public enum BattleSquareState { Closed, Inhabited, Open, Selectable }
    public readonly static BattleSquareState[] AvailableSquareStates = new BattleSquareState[] { BattleSquareState.Open, BattleSquareState.Inhabited };
    [SerializeField] public BattleSquareState state { get; private set; }
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer selectionZone;
    [SerializeField] private SpriteRenderer healthBar;
    [SerializeField] private List<UnitController> inhabitedUnits = new List<UnitController>();

    [field: SerializeField] 
    public ElementalType activeElement { get; private set; }
    [SerializeField] private ElementalType baseElement;
    [SerializeField] private Animator baseAnimator;



    private void Start() {
        baseElement = GameManager.Hub.Map.GetElementAtLocation(Vector3Int.FloorToInt(transform.position));
        SetActiveElement(baseElement);
        baseAnimator.SetFloat("Offset", Random.Range(0f, 1f));
    } 

    private void OnTriggerEnter2D(Collider2D collision) {
        var unit = collision.GetComponent<UnitController>();
        if (!unit) return;

        baseAnimator.SetTrigger("Special");
        BattleGrid.InhabitedSquares.Add(this);
        UpdateHealthBar();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var unit = collision.GetComponent<UnitController>();
        if (!unit) return;
        
        inhabitedUnits.Remove(unit);
        if(!ContainsUnits()) 
            BattleGrid.InhabitedSquares.Remove(this);

        SetState(ContainsUnits() ? BattleSquareState.Inhabited : BattleSquareState.Open);
        UpdateHealthBar();
    }

    private void OnMouseEnter() {
        if (!BattleGrid.awaitingSquareSelection) return;
        if (state != BattleSquareState.Selectable) return;

        BattleGrid.Selected = this;
        EventsActive(true);
    }

    private void OnMouseExit() {
        if (!BattleGrid.awaitingSquareSelection) return;

        BattleGrid.Selected = null;
        EventsActive(false);
    }

    private void OnMouseUpAsButton() {
        if (!BattleGrid.awaitingSquareSelection) return;
        BattleGrid.SquareSelect();
    }

    public void SetState(BattleSquareState state, params UnitController[] units) {
        this.state = state;
        if (state == BattleSquareState.Selectable) {
            SetColor(inhabitedUnits.Count > 0 ? Color.red : Color.green);
        }
        else {
            SetColor(Color.white);
        }

        if (state != BattleSquareState.Inhabited || units.Length == 0)
            return;

        foreach (UnitController external in units)
        {
            if (!inhabitedUnits.Contains(external))
            {
                external.pathFinder.GoTo(transform);
                inhabitedUnits.Add(external);
            }
        }

    }

    public void SetActiveElement(ElementalType element) { 
        if(activeElement == element) {
            baseAnimator.SetTrigger("Special");
            return;
        }

        activeElement = element;

        for (int i = 0; i < baseAnimator.layerCount; i++) {
            baseAnimator.SetLayerWeight(i, (i - 1) == (int)activeElement ? 1f : 0f);
        }

        baseAnimator.SetTrigger("Reset");

        BattleSquare[] neighbours = BattleGrid.Radial(this);
        foreach (BattleSquare n in neighbours) {
            ElementalType combine = Elemental.Combine(n.activeElement, element);
            if (combine == n.activeElement) continue;

            n.SetActiveElement(combine);
        }
    }

    public BattleSquare GetNeighbour() {
        Vector2 squareCoordinates = BattleGrid.Coordinates(this);
        return BattleGrid.ClosestSquare(squareCoordinates);
    }

    public UnitController[] GetInhabitedUnits() {
        return inhabitedUnits.ToArray();
    }

    public bool ContainsUnits(params UnitController[] units) {
        if (units.Length == 0) return inhabitedUnits.Count > 0;

        foreach (UnitController external in units) {
            foreach (UnitController inhabited in inhabitedUnits) {
                if (inhabited.Equals(external)) return true;
            }
        }

        return false;
    }

    public void SetColor(Color color) {
        if (color.Equals(Color.red) || color.Equals(Color.green)) {
            selectionZone.color = color;
            selectionZone.gameObject.SetActive(true);
            LeanTween.color(selectionZone.gameObject, (color/2), 1f).setLoopPingPong();
            return;
        }

        selectionZone.gameObject.SetActive(false);
        LeanTween.cancel(selectionZone.gameObject);
        spriteRenderer.color = color;
    }

    public void UpdateHealthBar() {
        healthBar.gameObject.SetActive(state == BattleSquareState.Inhabited);
        LeanTween.cancel(healthBar.gameObject);
        if (state != BattleSquareState.Inhabited) return;

        float y = inhabitedUnits[0].combatant.PercentHP();
        healthBar.transform.localScale = new Vector2(healthBar.transform.localScale.x, (0.81656f * y));

        bool faceRight = inhabitedUnits[0].transform.localScale.x < 0;
        healthBar.transform.localPosition = new Vector2((faceRight ? hpOffset : -hpOffset), healthBar.transform.localPosition.y);

        if (y > 0.2f) return;
        
        LeanTween.color(healthBar.gameObject, 
            new Color(healthBar.color.r, healthBar.color.g, healthBar.color.b, 0.2f), 1f).setLoopPingPong();
    }

    private void EventsActive(bool active) {
        if (!active) {
            BattleEvent.TakenDamageEvent -= UpdateHealthBar;
            BattleEvent.TakenHealEvent -= UpdateHealthBar;
            return;
        }
        BattleEvent.TakenDamageEvent += UpdateHealthBar;
        BattleEvent.TakenHealEvent += UpdateHealthBar;
    } 

    public override string ToString() {
        return $"{name} | {state}";
    }

    
}
