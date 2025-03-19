class DeadState : State
{
    public DeadState(EnemyController controller) : base(controller) { }

    public override void Run()
    {
        _controller.Die();
    }
}