public interface IAbility
{
    string Name { get; }
    bool IsAvailable { get; }
    bool IsUpgraded { get; }
    float Cooldown { get; }

    bool Activate();
    void Update(bool IsControllable);
    void Upgrade();
}