using System.Collections;
using UnityEngine;

public class SpellOutTextEffect : TextEffect
{
    [SerializeField, Min(1)] private int delay;
    private Coroutine effect;

    public override void StartEffect() {
        if (string.IsNullOrEmpty(buffer)) return;
        Enable();
        if (effect != null) StopCoroutine(effect);
        effect = StartCoroutine(AddChars());
    }

    public override void StartEffect(string buffer) {
        SetText(buffer);
        StartEffect();
    }

    public override void SetText(string buffer)  {
        base.SetText(buffer);
        txtObj.text = string.Empty;
    }


    IEnumerator AddChars() {
        txtObj.text = string.Empty;
        for (int i = 0; i < buffer.Length; i++) {
            txtObj.text += buffer[i];
            yield return new WaitForSeconds((float)delay / 100);
        }
    }

}
