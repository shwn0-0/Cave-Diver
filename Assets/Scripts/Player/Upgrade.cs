public enum Upgrade
{
    BoostAbility,
    ShieldAbility,
    LureAbility,
    C4Ability,
    AttackDamage_MoveSpeed,
    Health_Shield,
}

public static class UpgradeExtensions
{
    public static bool IsAbilityUpgrade(this Upgrade upgrade)
    {
        return upgrade == Upgrade.BoostAbility
            || upgrade == Upgrade.ShieldAbility
            || upgrade == Upgrade.LureAbility
            || upgrade == Upgrade.C4Ability;
    }
}