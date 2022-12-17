using UnityEngine;

public class GUIController : MonoBehaviour
{
    public delegate void Refresh();
    public static event Refresh RefreshEvent;
    public static void InvokeRefreshEvent() => RefreshEvent.Invoke();

}
