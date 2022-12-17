using UnityEngine;
using UnityEngine.UI;

public class BattleSidePanelUI : UIComponent
{
    [SerializeField] private Image Bookmark;
    [SerializeField] private Image BookmarkRadius;
    [SerializeField] private Button[] Buttons;
    
    private float relativeXPosition => Screen.width * 0.1f;



    private void FixedUpdate()
    {
        if (!gameObject.activeSelf) return;

        AlterVisability(Mathf.Clamp(relativeXPosition / Input.mousePosition.x, 0f, 1f));
    }


    public override void Enable()
    {
        base.Enable();
        AlterVisability(0);
    }

    public override void Disable()
    {
        base.Disable();
        AlterVisability(1);
    }

    private void AlterVisability(float progress) {

        Debug.Log($"Buttons enabled: {progress >= 0.2f}");

        foreach (Button b in Buttons) {
            //Should button be turned on?
            b.gameObject.SetActive(progress >= 0.2f);
            if (progress < 0.2f) continue;

            //Fade button based on location
            b.image.color = new Color(1, 1, 1, progress + 0.01f);
        }

        Bookmark.color = new Color(1, 0.2f, 0.2f, 1 - (progress + 0.01f));
        BookmarkRadius.color = new Color(0, 0, 0, (progress + 0.01f) / 3);
    }

}
