using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CameraController : MonoBehaviour
{
    public PixelPerfectCamera PPCamera => GetComponent<PixelPerfectCamera>();
    public Camera Camera => GetComponent<Camera>();
    public static CameraController main { get; private set; }
    private void Awake() => main = this;

    public Transform target;
    [Range(0, 10)] 
    public float speed = 1f;
    public bool cameraLock = false;
    public bool cameraZoom { get; private set; }


    private void FixedUpdate()
    {
        if (!target || cameraLock) return;

        float distance = Vector2.Distance(target.position, transform.position);
        float x = Mathf.Lerp(transform.position.x, target.position.x, distance * (speed * Time.deltaTime));
        float y = Mathf.Lerp(transform.position.y, target.position.y, distance * (speed * Time.deltaTime));
        transform.position = new Vector3(x, y, -20);
    }

    public void Go(Transform target, float time) {
        if (!target) return;

        LeanTween.move(gameObject, target.position - Vector3.forward, time).setEaseInOutElastic();
    }

    public void ZoomCamera(bool zoom) {
        cameraZoom = zoom;
        if (cameraZoom) {
            PPCamera.refResolutionX = 320;
            PPCamera.refResolutionY = 180;
            return;
        }

        PPCamera.refResolutionX = 640;
        PPCamera.refResolutionY = 360;
    }

    public void CenterBounds(IEnumerable<Component> objects) {
        var b = GameManager.Units.UnitManager.Player.GetComponent<CircleCollider2D>().bounds;
        foreach (Component obj in objects)
        {
            b.Encapsulate(obj.GetComponent<CircleCollider2D>().bounds);
        }
        SetPosition(b.center);
    }

    public void SetPosition(Vector2 location) => transform.position = new Vector3(location.x, location.y, -10);

    [ContextMenu("ToggleZoom")]
    private void ToggleZoom() => ZoomCamera(!cameraZoom);
}
