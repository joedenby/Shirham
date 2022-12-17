using UnityEngine;

[System.Serializable]
public struct GUIAnchor
{
    public Vector2 offset;
    public AnchorPoint anchorPoint;
    public enum AnchorPoint { 
        None, Center, Top, Bottom, Left, Right, BottomLeft, BottomRight, TopLeft, TopRight
    }

    public Vector2 WorldPosition() {
        var cam = CameraController.main.Camera;
        Vector2 pos = Vector2.zero;

        switch (anchorPoint) {
            default:
                return pos + offset;
            case AnchorPoint.Center:
                pos = cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
                return pos + offset;
            case AnchorPoint.Bottom:
                pos = cam.ScreenToWorldPoint(new Vector2(Screen.height / 2, 0));
                Debug.Log($"POS {pos} Screen: {new Vector2(Screen.width, Screen.height)}");
                return pos + offset;
            case AnchorPoint.BottomLeft:
                pos = cam.ScreenToWorldPoint(new Vector3(0, 0, 0));
                return pos + offset;
            case AnchorPoint.BottomRight:
                pos = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));
                return pos + offset;
            case AnchorPoint.Right:
                pos = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height / 2, 0));
                return pos + offset;
            case AnchorPoint.TopRight:
                pos = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
                return pos + offset;
            case AnchorPoint.Top:
                pos = cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height, 0));
                return pos + offset;
            case AnchorPoint.TopLeft:
                pos = cam.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
                return pos + offset;
        }
    }
}
