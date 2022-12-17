using GameManager.Hub;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfirmationWindow : UIWindow
{
    public static ConfirmationWindow Instance { get; private set; }
    private static GameObject Prefab;
    public delegate void Option1();
    public delegate void Option2();

    [SerializeField]
    private Button[] buttons;
    [SerializeField]
    private TextMeshProUGUI[] buttonTMPs;
    [SerializeField]
    private TextMeshProUGUI header, body;


    public static ConfirmationWindow CreateInstance() {
        if (!Prefab) AssignPrefab();
        DestroyWindow();
        if (!Prefab) {
            Debug.LogError("No ConfirmationWindow prefab object was found in Resources.");
            return null;
        }
        Instance = Instantiate(Prefab, UI.GUI.transform).GetComponent<ConfirmationWindow>();

        return Instance;
    }

    public static void DestroyWindow() {
        if (!Instance) return;

        Time.timeScale = 1.0f;
        DestroyImmediate(Instance.gameObject);
    }

    public void Set(string headerTxt, string bodyTxt, Option1 option, string buttonTxt, Option2 option2 = null, string button2Txt = "Button") { 
        header.text = headerTxt;
        body.text = bodyTxt;
        buttons[0].gameObject.SetActive(option != null);
        buttons[1].gameObject.SetActive(option2 != null);

        SetWindowPosition();
        Time.timeScale = 0;

        //Set button 1
        if (option == null) return;
        buttons[0].onClick.RemoveAllListeners();
        buttons[0].onClick.AddListener(delegate { option(); });
        buttons[0].onClick.AddListener(delegate { DestroyWindow(); });
        buttonTMPs[0].text = buttonTxt;

        //Set button 2
        if (option2 == null) return;
        buttons[1].onClick.RemoveAllListeners();
        buttons[1].onClick.AddListener(delegate { option2(); });
        buttons[1].onClick.AddListener(delegate { DestroyWindow(); });
        buttonTMPs[1].text = button2Txt;
    }

    private void SetWindowPosition() { 
        if(!Instance) return;

        Camera cam = CameraController.main.Camera;
        Vector2 center = cam.ScreenToWorldPoint(new Vector2(Screen.width * 0.6f, Screen.height * 0.5f));
        transform.position = center;
    }

    private static void AssignPrefab() {
        if (Prefab) {
            Debug.Log("Prefab exits already.");
            return;
        }

        Prefab = Resources.Load<GameObject>("GUI/ConfirmationWindow");
    }
}
