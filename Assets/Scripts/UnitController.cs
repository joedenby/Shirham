using GameManager.Units;
using GameManager.Hub;
using UnityEngine;


[RequireComponent(typeof(AIPathFinder))]
public class UnitController : MonoBehaviour
{
    private Animator animator;
    public AIPathFinder pathFinder => GetComponent<AIPathFinder>();
    public UnitRoute unitRoute = new ();
    public Combatant combatant;
    private new Rigidbody2D rigidbody2D;
    
    public float moveSpeed => 4 - combatant.FinalStats().EndAsPercent();
    public bool isMoving => desiredLocation != (Vector2)transform.position;
    [SerializeField] private Vector2 desiredLocation;
    public bool movementOverride;



    protected virtual void Start() {
        Initialize();
        SetMaxHP();
    }

    private void FixedUpdate() {
        Pathing(); 
        Animate();
    }

    public void Initialize()
    {
        animator = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.freezeRotation = true;

        var coll = gameObject.AddComponent<CircleCollider2D>();
        coll.radius = 0.33f;
        coll.offset = (Vector2.up / 4);
        coll.isTrigger = true;

        desiredLocation = transform.position;
    }

    public Vector2 NextVectorMoveOverride() { 
        if (isMoving) return desiredLocation;
        var direction = GameManager.InputManager.PlayerInput().Move.ReadValue<Vector2>();
        if (Mathf.Abs(direction.x) == Mathf.Abs(direction.y)) {
            var x = (direction.x > 0 ? 1f : -1f);
            var y = (direction.y > 0 ? 1 : -1f);
            direction = new Vector2(x, y);
        }

        var v = (Vector2)transform.position + direction;
        return Navigation.ObstacleAtLocation(v) ? desiredLocation : v;
    }

    protected void Pathing()
    {
        if (movementOverride) {
            desiredLocation = NextVectorMoveOverride();
        } 
        else if (desiredLocation == (Vector2)transform.position) {
            if (pathFinder.hasPath)
            {
                desiredLocation = pathFinder.NextPoint();
            }
            else if (IsPlayer())
            {
                var wayPoint = Navigation.pointObj;
                if (wayPoint && AIPathGrid.main.SameLocation(transform.position, wayPoint.transform.position))
                {
                    Navigation.DestoyWayPoint();

                    if (pathFinder.targetLock) return;
                    pathFinder.Clear();
                }
            }
            else if(!unitRoute.waiting) {
                var pos = unitRoute.GetNext();
                pathFinder.GoTo(pos == Vector2.zero ? desiredLocation : pos);
            }     
            return;
        }
       
        transform.position = Vector2.MoveTowards(transform.position, desiredLocation, moveSpeed * Time.deltaTime);
    }

    public void FaceLocation(Vector2 direction) {
        bool left = transform.position.x - direction.x > 0;
        transform.localScale = new Vector2(left ? 1 : -1, 1);
    }

    public void StopMoving() {
        if (IsPlayer()) Navigation.DestoyWayPoint();

        float x = Mathf.Floor(transform.position.x) + 0.5f;
        float y = Mathf.Floor(transform.position.y) + 0.5f;

        desiredLocation = new Vector2(x, y);
    }

    public bool IsPlayer()  {
        if (!UnitManager.Player) return false;
        return UnitManager.Player.Equals(this);
    }

    public bool IsEnemy() {
        return GetType() == typeof(EnemyUnit);
    }

    public void AnimationTrigger(string request) => animator.SetTrigger(request);

    public bool OppositeUnitType(UnitController unit) => 
        (unit.IsEnemy() && !IsEnemy()) || (IsEnemy() && !unit.IsEnemy());

    protected void Animate() {
        if (combatant.isDead) {
            AnimationTrigger("Die");
            enabled = false;
            return;
        }

        animator.SetFloat("RunState", isMoving ? 0.5f : 0);
        if (!isMoving) return;

        int flip = (int)((Vector2)transform.position - desiredLocation).normalized.x;
        if (flip == 0) return;
        transform.localScale = new Vector2(flip, 1);
    }

    [ContextMenu("SetMaxHp")]
    private void SetMaxHP() {
        combatant.HP = combatant.MaxHP();
    }
    
}
