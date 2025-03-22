class DeadState : State
{    
    public override void Run(EnemyController controller)
    {
        controller.Die();
    }
}