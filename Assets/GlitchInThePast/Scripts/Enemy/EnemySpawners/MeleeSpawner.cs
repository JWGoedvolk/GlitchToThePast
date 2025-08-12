namespace Systems.Enemies
{
    /// <summary>
    /// Author: JW
    /// This is a spawner for the melee enemy using the new object pooling stuff. It is just for inheritance to separate melee and ranged spawning
    /// </summary>
    public class MeleeSpawner : PooledEnemySpawner
    {
        // This is for the sake of inheritance. nothing changes from the PooledSpawner logic here
    }
}