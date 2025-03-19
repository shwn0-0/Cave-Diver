class AttackState : State
{
    public AttackState(EnemyController controller) : base(controller) { }

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
        else if (_controller.FinishedAttacking)
        {
            _controller.ChangeStates(new IdleState(_controller));
        }
        else
        {
            _controller.BeAttacking();
        }
    }
}