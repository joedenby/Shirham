using UnityEngine.UI;
using UnityEngine;

public class PlatformButton : UIComponent
{
    private Button button;
    private Vector2 enabledPos;
    private AudioSource audioSource;
    [SerializeField] private Vector2 disabledPos;
    


    private void Start()
    {
        button = GetComponent<Button>();
        audioSource = GetComponent<AudioSource>();
        enabledPos = transform.localPosition;
        Disable();
    }

    protected override void MouseEnter() => Enable();
    protected override void MouseExit() => Disable();

    public override void Enable()
    {
        button.interactable = true;
        LeanTween.moveLocal(gameObject, enabledPos, 0.2f);
        if (audioSource.isPlaying) return;

        audioSource.pitch = Random.Range(0.8f, 1);
        audioSource.Play();
    }

    public override void Disable()
    {
        button.interactable = false;
        LeanTween.moveLocal(gameObject, disabledPos, 0.2f);
    }

   

}
