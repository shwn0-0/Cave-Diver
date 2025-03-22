class State
{
    public static State Idle { get; } = new IdleState();
    public static State Attack { get; } = new AttackState();
    public static State Running { get; } = new RunningState();
    public static State Stunned { get; } = new StunnedState();
    public static State Dead { get; } = new DeadState();

    public virtual void Run(EnemyController controller) {}
}