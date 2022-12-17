using UnityEngine.UI;
using UnityEngine;

public class GUIBar : MonoBehaviour
{
    [Min(0)]public float value = 1;
    public float speed = 1;
    private float speedRelative => 0.05f + (speed * Mathf.Abs(front.fillAmount - intermediary.fillAmount));

    [SerializeField]
    private Color[] gradient = new Color[0];
    [SerializeField]
    private Image intermediary, front;


    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateBar();
    }

    private void UpdateBar()
    {
        front.fillAmount = front.fillAmount < value ? 
            front.fillAmount + (Time.deltaTime * speedRelative) : value;


        intermediary.fillAmount = intermediary.fillAmount > value ? 
             intermediary.fillAmount - (Time.deltaTime * speedRelative) : value;

        if (gradient.Length == 0) return;

        int i = (gradient.Length - 1) - Mathf.Clamp(Mathf.CeilToInt(value * gradient.Length), 0, gradient.Length-1);
        front.color = Color.Lerp(front.color, gradient[i], (front.fillAmount / value));

        Debug.Log($"i: {i} f: {(front.fillAmount / value)} \nColor({front.color})");
    }
}
