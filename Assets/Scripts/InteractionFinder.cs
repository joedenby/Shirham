using UnityEngine;

public class InteractionFinder : MonoBehaviour
{
    public Interactable Focus { get; private set; }
    public static InteractionFinder main { get; private set; }



    private void Awake()
    {
        if (main)
        {
            Destroy(gameObject);
            return;
        }
        main = this;
    }

    private void Update()
    {
        if(!Input.GetKeyDown(KeyCode.Mouse0)) return;
        if(!Focus) return;

        Focus.MouseDown();
    }

    private void FixedUpdate()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos = new Vector2(Mathf.Floor(pos.x) + 0.5f, Mathf.Floor(pos.y) + 0.5f);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, 0.33f);
        if (colliders.Length == 0) {
            Focus = null;
            return;
        }

        Interactable interactable = null;
        foreach (Collider2D coll in colliders) {
            interactable = coll.GetComponent<Interactable>();
            if (interactable) break;
        }
        if (!interactable) {
            if (Focus) {
                Focus.MouseExit();
                Focus = null;
            }

            return;
        }

        interactable.MouseEnter();
        Focus = interactable;
    }

}
