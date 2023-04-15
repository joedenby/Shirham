using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MinigameBetBox : MonoBehaviour
{
    public TextMeshProUGUI valueText;
    public string suffix;

    public DragonEyesGUI dragonEyes;


    public void UpdateBet(int index)
    {
        DragonEyesMinigame minigame = dragonEyes.minigame;

        int mod = (int)Input.mouseScrollDelta.y;
        int value = (index == 0) ? minigame.playerCash.Gold : (index == 1) ? minigame.playerCash.Silver : minigame.playerCash.Copper;
        int betValue = (index == 0) ? minigame.bet.Gold : (index == 1) ? minigame.bet.Silver : minigame.bet.Copper;
        int total = betValue + mod;

        if (total > value || total < 0) return;
        var bet = minigame.bet;
        if (index == 0)
        {
            minigame.bet = new Wallet(total, bet.Silver, bet.Copper);
        }
        else if (index == 1)
        {
            minigame.bet = new Wallet(bet.Gold, total, bet.Copper);
        }
        else
        {
            minigame.bet = new Wallet(bet.Gold, bet.Silver, total);
        }

        valueText.text = $"{betValue}{suffix}";
    }

}
