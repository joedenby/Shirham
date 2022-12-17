public interface IModule
{
    public float Confidence();
    public bool IsActive();
    public void PerformAction();

    public void SetEnemyUnit(EnemyUnit unit);
}
