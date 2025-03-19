class IdleState : State
{
    public IdleState(EnemyController controller) : base(controller) { }

    public override void Run()
    {
        if (_controller.IsDead)
        {
            _controller.ChangeStates(new DeadState(_controller));
        }
        else if (_controller.IsStunned)
        {
            _controller.ChangeStates(new StunnedState(_controller));
        }
        else if (_controller.IsTargetInRange)
        {
            _controller.ChangeStates(new AttackState(_controller));
        }
        else
        {
            _controller.ChangeStates(new RunningState(_controller));
        }
    }
}