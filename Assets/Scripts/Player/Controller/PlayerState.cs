namespace Player.Controller
{
    public enum PlayerState
    {
        Dead,
        Spawning,
        Alive,
        Dying
    }

    public enum EnemyState
    {
        Dead,
        StartingSpawn,
        Spawning,
        EndingSpawn,
        Alive,
        Dying
    }

    public enum BossState
    {
        Dead,
        Spawning,
        MovingDown,
        StartingPhaseOne,
        PhaseOne,
        StartingPhaseTwo,
        PhaseTwo,
        Dying,
    }


}