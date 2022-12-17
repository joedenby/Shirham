using UnityEngine;

public class Door : Interactable
{
    public Sprite closedSprite;
    public Sprite openSprite;
    public bool locked = false;
    private SpriteRenderer SpriteRenderer => GetComponent<SpriteRenderer>();


    private void Start() { 
        if(SpriteRenderer) SpriteRenderer.sprite = closedSprite;

        var coll = gameObject.AddComponent<BoxCollider2D>();
        coll.size = new (0.5f, 1f);
        coll.offset = new (0, -0.5f);
        coll.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var unit = collision.GetComponent<UnitController>();
        if (!unit) return;
        //TODO: Display transition UI here
        if (!SpriteRenderer || locked) return;
        SpriteRenderer.sprite = openSprite;
        if (unit.IsPlayer()) Prompt(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var unit = collision.GetComponent<UnitController>();
        if (!unit) return;
        //TODO: Hide transition UI here
        if (!SpriteRenderer || locked) return;
        SpriteRenderer.sprite = closedSprite;

        if(unit.IsPlayer()) Prompt(false);
    }
}
