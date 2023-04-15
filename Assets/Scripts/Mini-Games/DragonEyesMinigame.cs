using Pathfinding.Ionic.Zlib;
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
        Wallet w1 = new Wallet(5, 50, 0); //55000
        Wallet w2 = new Wallet(0, 75, 0); //7500

        Debug.Log($"w1: {w1} - w2: {w2} = {w1 - w2}");

    }
}
