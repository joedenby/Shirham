using GameManager.Battle;
using UnityEngine.UI;
using UnityEngine;

public class BattleUnitCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private RawImage image;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();

        // Only render objects in Unit layer (6)
        cam.cullingMask = 1 << 6;
    }

    public void SetTarget(UnitController unit) {
        if (!unit) return;
        if (target) SetTransformLayer(target, false);

        target = unit.transform;
        SetTransformLayer(target.GetChild(0), true);
    }

    public void SetOutputImage(RawImage image) => this.image = image;

    private void SetTransformLayer(Transform parent, bool active) 
    {
        foreach (Transform child in parent) {
            SetTransformLayer(child, active);
        }

        parent.gameObject.layer = active ? 6 : 0;
    }

    private void Update()
    {
        if (!target || !image) return;
        image.transform.localScale = new Vector3(-target.localScale.x, 1, 1);
        transform.position = new Vector3(target.position.x, target.position.y + 0.5f, -10);
    }

    private void OnDisable()
    {
        if (!target) return;
        SetTransformLayer(target, false);
        target = null;
    }
}
