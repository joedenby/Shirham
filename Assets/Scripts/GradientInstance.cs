using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class GradientInstance : UIComponent
{
    private Material material;


    private void Start()
    {
        material = GetComponent<SpriteRenderer>().material;
        if (!material) {
            Debug.LogError($"No material found for object {name}");
            gameObject.SetActive(false);
        }

    }

    private void FixedUpdate()
    {
        if (!GradientController.main) return;
        
        material.SetFloat("_Origin", GradientController.main.Origin);
        material.SetColor("_TopColor", GradientController.main.color1);
        material.SetColor("_BottomColor", GradientController.main.color2);
    }

}
