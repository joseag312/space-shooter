using Godot;

[GlobalClass]
public partial class WeaponInstanceData : Resource
{
    [Export] public string Key { get; set; } = "";

    [Export] public int CurrentAmount { get; set; } = 0;
    [Export] public float CooldownRemaining { get; set; } = 0f;

    [Export] public int OverrideDamage { get; set; } = -1;
    [Export] public int OverrideDamagePercentage { get; set; } = -1;
    [Export] public float OverrideCooldownTime { get; set; } = -1f;

    public bool HasOverrideDamage => OverrideDamage >= 0;
    public bool HasOverrideDamagePercentage => OverrideDamagePercentage >= 0;
    public bool HasOverrideCooldown => OverrideCooldownTime >= 0f;

    public WeaponInstanceData() { }

    public WeaponInstanceData(string key)
    {
        Key = key;
    }

    // Optional: helpers to resolve values with base data
    public int GetEffectiveDamage(WeaponDataComponent baseData)
        => HasOverrideDamage ? OverrideDamage : baseData.Damage;

    public int GetEffectiveDamagePercentage(WeaponDataComponent baseData)
        => HasOverrideDamagePercentage ? OverrideDamagePercentage : baseData.DamagePercentage;

    public float GetEffectiveCooldown(WeaponDataComponent baseData)
        => HasOverrideCooldown ? OverrideCooldownTime : baseData.CooldownTime;
}