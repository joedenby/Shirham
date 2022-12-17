using UnityEngine;

public class UIWindow : UIComponent
{
    private SpriteRenderer border;
    private SpriteRenderer fill;
    protected Animator animator => GetComponent<Animator>();
    public Vector2 size = Vector2.zero;


    private void Start() {
        border = GetComponent<SpriteRenderer>();
        fill = transform.GetChild(0).GetComponent<SpriteRenderer>();
        if (border) size = border.size;
    }

    private void OnEnable() => Enable();

    private void Update() {
        border.size = size;
        fill.size = new Vector2(size.x - 0.1f, size.y - 0.1f);
    }

    public override void Enable() {
        base.Enable();
        SetAnchorPosition();
        transform.localScale = Vector2.one * 1.1f;
        LeanTween.scale(gameObject, Vector2.one, 0.5f).setEaseOutBounce().setIgnoreTimeScale(true);
    } 

}
