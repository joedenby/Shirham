using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class InteractionPrompt : MonoBehaviour
{
    public Sprite[] Icons;
    [SerializeField] private Image promptIcon;
    [SerializeField] private TextMeshProUGUI promptTxt;
    [SerializeField] private RectTransform promptBackdrop;
    private Transform parent;

    public void ShowPrompt(int iconIndex, string prompt, Transform parent)
    {
        int index = iconIndex > 0 && iconIndex < (Icons.Length - 1) ? iconIndex : 0;
        promptIcon.sprite = Icons[index];
        promptTxt.text = prompt;
        this.parent = parent;

        Resize();
        Position();
    }

    public void ShowPrompt(Sprite icon, string prompt, Transform parent) {
        promptIcon.sprite = icon;
        promptTxt.text = prompt;
        this.parent = parent;

        Resize();
        Position();
    }

    private void Position() => transform.position = ((Vector2)parent.transform.position + Vector2.up);

    public void Resize() {
        if(!promptTxt) promptTxt = GetComponent<TextMeshProUGUI>();
        float promptLength = promptTxt.text.Length;
        float size = 1 + (0.24f * promptLength);

        promptBackdrop.sizeDelta = new Vector2(size, 1);
    }

}
