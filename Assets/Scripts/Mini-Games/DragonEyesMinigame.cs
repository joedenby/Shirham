using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonEyesMinigame : MonoBehaviour , IMinigame
{
    private int[] playerWallet = new int[5]; 
    private int potAmount;
    
    public MinigameState State => UpdateState();

    public void EndGame()
    {
        throw new System.NotImplementedException();
    }

    public void StartGame()
    {
        for (int i = 0; i < playerWallet.Length; i++)
        {
            playerWallet[i] = 2;
        }
        potAmount = 0;
    }

    private MinigameState UpdateState()
    {
        return MinigameState.Inactive;
    }

    [ContextMenu("test")]
    public void test()
    {
        var w1 = new Wallet(79 , 67 , 7);
        var w2 = new Wallet(35, 53, 0);

        Debug.Log($"a[{w1}] - b[{w2}] = [{w1 - w2}]");
    }
}
