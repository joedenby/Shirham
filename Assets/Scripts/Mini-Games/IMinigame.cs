public interface IMinigame 
{
    public MinigameState State { get; }

    public void StartGame();
    public void EndGame();

 
}
