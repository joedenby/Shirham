using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DragonEyesGUI : MonoBehaviour
{
    public DragonEyesMinigame minigame;

    [Header("BetWindow")]
    public GameObject betWindow;
    public TextMeshProUGUI walletText;
    public MinigameBetBox[] betBoxes;

    public UnityEvent UIUpdate = new UnityEvent();

    public void UpdateGUI() => UIUpdate.Invoke();

    public void UpdateBetWindow() {
        Wallet display = minigame.playerCash - minigame.bet;

        walletText.text = $"[{display.Gold}g {display.Silver}s {display.Copper}c]";
    }

}
