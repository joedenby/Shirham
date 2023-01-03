using UnityEngine;

[System.Serializable]
public struct GUIAnchor
{
    public Vector2Int offset;
    public AnchorPoint anchorPoint;
    public enum AnchorPoint { 
        None, Center, Top, Bottom, Left, Right, BottomLeft, BottomRight, TopLeft, TopRight
    }
    private int refX => CameraController.main.Camera.refResolutionX / 32;
    private int refY => CameraController.main.Camera.refResolutionY / 32;

    public Vector2 WorldPosition() {
        Vector2 pos = (Vector2)CameraController.main.transform.position;

        switch (anchorPoint) {
            default:
                return pos + offset;
            case AnchorPoint.Bottom:
                return pos + (Vector2.down * (refY / 2)) + offset;
            case AnchorPoint.BottomLeft:
                pos += (Vector2.down * (refY / 2));
                return pos + (Vector2.left * refX / 2) + offset;
            case AnchorPoint.BottomRight:
                Debug.Log($"Pos: {pos} " +
                 $"\nDown: {(Vector2.down * (refY / 2))} " +
                 $"\nOffset: {offset}");
                pos += (Vector2.down * (refY / 2));
                return pos + (Vector2.right * refX / 2) + offset;
            case AnchorPoint.Right:
                return pos + (Vector2.right * refX / 2) + offset;
            case AnchorPoint.TopRight:
                pos += (Vector2.up * (refY / 2));
                return pos + (Vector2.right * refX / 2) + offset;
            case AnchorPoint.Top:
                return pos + (Vector2.up * (refY / 2)) + offset;
            case AnchorPoint.TopLeft:
                pos += (Vector2.up * (refY / 2));
                return pos + (Vector2.left * refX / 2) + offset;
        }
    }
}
