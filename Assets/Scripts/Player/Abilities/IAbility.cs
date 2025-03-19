interface IAbility
{
    string Name { get; }
    bool IsAvailable { get; }
    bool IsUpgraded { get; }

    bool Activate();
    void Update();
    void Upgrade();
}