using System.Diagnostics.Contracts;
using UnityEngine;

public class OutlineController : MonoBehaviour
{
    [SerializeField] 
    private SpriteRenderer[] renderers;
    private Material[] materials;


    private void Awake()
    {
        if (renderers.Length == 0) {
            Debug.LogWarning($"{name} has no renderers assigned for outline.");
            return;
        }

        SetMaterials();
    }

    private void SetMaterials() {
        materials = new Material[renderers.Length];

        for (int i = 0; i < renderers.Length; i++) {
            if (renderers[i] is null) continue;
            
            var originalMaterial = renderers[i].sharedMaterial;
            materials[i] = new Material(originalMaterial);
            renderers[i].material = materials[i];
        }
    }

    public void SetOutline(Color color, float thickness = 1f)
    {
        if (materials.Length != renderers.Length) {
            Debug.Log("Change in assigned renderers detected. " +
                "Setting new materials.");
            SetMaterials();
        }

        foreach (var mat in materials) {
            mat.SetFloat("_OutlineThickness", thickness);
            mat.SetColor("_OutlineColor", color);
        }
    }
}
