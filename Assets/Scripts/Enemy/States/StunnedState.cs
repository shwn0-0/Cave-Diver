class StunnedState : State
{
    public override void Run(EnemyController controller)
    {
        if (controller.IsDead)
        {
            controller.ChangeStates(Dead);
        }
        else if (!controller.IsStunned)
        {
            controller.ChangeStates(Idle);
        }
        else
        {
            controller.BeStunned();
        }
    }
}