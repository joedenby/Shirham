using UnityEngine;
using GameManager.Hub;
using GameManager.Units;

public class Interactable : MonoBehaviour
{
    [SerializeField] protected Sprite actionIcon;
    [SerializeField] protected string promptText;
    [SerializeField] protected bool showPrompt = true;
    [SerializeField] protected bool approach = true;
    protected GameObject promptObj;



    protected void Prompt(bool active) {
        if(promptObj) Destroy(promptObj);
        if (!active || !showPrompt) return;

        promptObj = Instantiate(Resources.Load<GameObject>("Misc/Prompt"));
        var prompt = promptObj.GetComponent<InteractionPrompt>();
        prompt.ShowPrompt(actionIcon, promptText, transform); 
    }

    public virtual void MouseEnter() => Prompt(true);

    public virtual void MouseExit() => Prompt(false);

    public virtual void MouseDown() {
        Navigation.DestoyWayPoint();
        if (!approach) return;

        UnitManager.Player.pathFinder.GoTo(NeighbouringPosition());
    } 

    private Vector2 NeighbouringPosition() {
        Vector2[] radial = new Vector2[] {
            Vector2.up, Vector2.down, Vector2.left, Vector2.right
        };

        Vector2 choice = ((Vector2)transform.position + Vector2.down);
        foreach (Vector2 dir in radial) {
            var v = ((Vector2)transform.position + dir);
            if (Navigation.ColliderAtLocation(v)) continue;
            if (Vector2.Distance(transform.position, choice) > Vector2.Distance(transform.position, v)) { 
                choice = v;
            }
        }

        return choice;
    }

}
