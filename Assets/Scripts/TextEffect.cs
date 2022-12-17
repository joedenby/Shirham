using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Text))]
public class TextEffect : MonoBehaviour
{
    [SerializeField] protected Text txtObj;
    [SerializeField] protected string buffer;
    [SerializeField] private bool autoStart = true;

    private void Start() {
        txtObj = GetComponent<Text>();
        if (autoStart) StartEffect();
    }

    public virtual void StartEffect() {
    }

    public virtual void StartEffect(string buffer) {
        SetText(buffer);
    }

    public virtual void SetText(string buffer) {
        this.buffer = buffer;
    }

    public virtual void Enable() {
        gameObject.SetActive(true);
    }

    public virtual void Disable() {
        gameObject.SetActive(false);
    }

}
