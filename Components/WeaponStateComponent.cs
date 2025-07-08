using Godot;

[GlobalClass]
public partial class WeaponStateComponent : Resource
{
    public WeaponDataComponent BaseData { get; set; }

    [Export] public string Key { get; set; } = "";
    [Export] public int CurrentAmount { get; set; } = 0;
    [Export] public float CooldownRemaining { get; set; } = 0f;
    [Export] public int OverrideDamage { get; set; } = -1;
    [Export] public int OverrideDamagePercentage { get; set; } = -1;
    [Export] public float OverrideCooldownTime { get; set; } = -1f;

    public bool HasOverrideDamage => OverrideDamage >= 0;
    public bool HasOverrideDamagePercentage => OverrideDamagePercentage >= 0;
    public bool HasOverrideCooldown => OverrideCooldownTime >= 0f;

    public WeaponStateComponent() { }

    public WeaponStateComponent(string key)
    {
        Key = key;
    }

    // Optional: helpers to resolve values with base data
    public int EffectiveDamage => HasOverrideDamage ? OverrideDamage : BaseData?.Damage ?? 0;
    public int EffectiveDamagePercentage => HasOverrideDamagePercentage ? OverrideDamagePercentage : BaseData?.DamagePercentage ?? 0;
    public float EffectiveCooldown => HasOverrideCooldown ? OverrideCooldownTime : BaseData?.CooldownTime ?? 0f;
}