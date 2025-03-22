class IdleState : State
{
    public override void Run(EnemyController controller)
    {
        if (controller.IsDead)
        {
            controller.ChangeStates(Dead);
        }
        else if (controller.IsStunned)
        {
            controller.ChangeStates(Stunned);
        }
        else if (controller.IsTargetInRange)
        {
            controller.ChangeStates(Attack);
        }
        else
        {
            controller.ChangeStates(Running);
        }
    }
}