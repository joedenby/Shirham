using UnityEngine;

public class GradientController : MonoBehaviour
{
    public static GradientController main { get; private set; }
    public float Origin { get; private set; }
    public float increment = 1;
    public Color color1;
    public Color color2;

    private void Awake() => main = this;

    private void FixedUpdate()
    {
        Origin += increment * Time.deltaTime;
        if (Origin < 1) return;
        ColorSwitch();
        Origin = -1;
    }

    private void ColorSwitch() {
        Color hold = color1;
        color1 = color2;
        color2 = hold;
    }

}
