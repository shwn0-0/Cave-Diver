class State
{
    protected readonly EnemyController _controller;

    protected State(EnemyController controller)
    {
        _controller = controller;
    }

    public virtual void Run() {}
}