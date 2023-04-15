using UnityEngine;

public class DragonEyesMinigame : MonoBehaviour , IMinigame
{
    private int[] playerTokens = new int[5]; 
    private int potAmount;
    public Wallet playerCash = new Wallet (5 ,5 ,5);
    public Wallet bet = new Wallet(0,0,0);
    
    public MinigameState State => UpdateState();
    public DragonEyesGUI gui;


    public void EndGame()
    {
        throw new System.NotImplementedException();
    }

    public void StartGame()
    {
        for (int i = 0; i < playerTokens.Length; i++)
        {
            playerTokens[i] = 2;
        }
        potAmount = 0;
        bet = new(0,0,0);
        playerCash = new Wallet(5, 5, 5);
        gui.UpdateGUI();
    }

    private MinigameState UpdateState()
    {
        return MinigameState.Inactive;
    }

    
}
