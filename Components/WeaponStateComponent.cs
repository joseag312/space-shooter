using Godot;

[GlobalClass]
public partial class WeaponStateComponent : Resource
{
    private const float MinimumCooldownTime = 0.4f;
    public WeaponDataComponent BaseData { get; set; }

    [Export] public string Key { get; set; } = "";
    [Export] public int CurrentAmount { get; set; } = 0;
    [Export] public float CooldownRemaining { get; set; } = 0f;

    // Overridable Properties
    [Export] public int OverrideSpeed { get; set; } = -1;
    [Export] public int OverrideDamage { get; set; } = -1;
    [Export] public int OverrideDamagePercentage { get; set; } = -1;
    [Export] public float OverrideCooldownTime { get; set; } = -1f;
    [Export] public int OverrideMaxAmount { get; set; } = -1;
    [Export] public bool OverrideUnlocked { get; set; } = false;
    [Export] public bool UseOverrideUnlocked { get; set; } = false;

    // Helper Properties
    public bool HasOverrideSpeed => OverrideSpeed >= 0;
    public bool HasOverrideDamage => OverrideDamage >= 0;
    public bool HasOverrideDamagePercentage => OverrideDamagePercentage >= 0;
    public bool HasOverrideCooldown => OverrideCooldownTime >= 0f;
    public bool HasOverrideMaxAmount => OverrideMaxAmount >= 0;
    public bool HasOverrideUnlocked => UseOverrideUnlocked;

    // Final Resolved Values
    public int EffectiveSpeed => HasOverrideSpeed ? OverrideSpeed : BaseData?.Speed ?? 0;
    public int EffectiveDamage => HasOverrideDamage ? OverrideDamage : BaseData?.Damage ?? 0;
    public int EffectiveDamagePercentage => HasOverrideDamagePercentage ? OverrideDamagePercentage : BaseData?.DamagePercentage ?? 0;
    public float EffectiveCooldown
    {
        get
        {
            float raw = HasOverrideCooldown ? OverrideCooldownTime : BaseData?.CooldownTime ?? 0f;
            return Mathf.Max(raw, MinimumCooldownTime);
        }
    }
    public int EffectiveMaxAmount => HasOverrideMaxAmount ? OverrideMaxAmount : BaseData?.MaxAmount ?? 0;
    public bool EffectiveUnlocked => HasOverrideUnlocked ? OverrideUnlocked : BaseData?.Unlocked ?? false;

    public WeaponStateComponent() { }

    public WeaponStateComponent(string key)
    {
        Key = key;
    }

    public override string ToString()
    {
        return
            $@"
            WeaponStateComponent {{
                Key: {Key}
                BaseData: {(BaseData != null ? BaseData.Key : "null")}
                SlotType: {BaseData?.SlotType}
                WeaponType: {BaseData?.Type}

                --- Effective Values ---
                Speed: {EffectiveSpeed} {(HasOverrideSpeed ? "(Override)" : "(Base)")}
                Damage: {EffectiveDamage} {(HasOverrideDamage ? "(Override)" : "(Base)")}
                Damage%: {EffectiveDamagePercentage} {(HasOverrideDamagePercentage ? "(Override)" : "(Base)")}
                Cooldown: {EffectiveCooldown:0.00}s {(HasOverrideCooldown ? "(Override)" : "(Base)")}
                MaxAmount: {EffectiveMaxAmount} {(HasOverrideMaxAmount ? "(Override)" : "(Base)")}
                Unlocked: {EffectiveUnlocked} {(HasOverrideUnlocked ? "(Override)" : "(Base)")}

                --- Current State ---
                CurrentAmount: {CurrentAmount}
                CooldownRemaining: {CooldownRemaining:0.00}s

                --- Overrides Raw ---
                OverrideSpeed: {OverrideSpeed}
                OverrideDamage: {OverrideDamage}
                OverrideDamagePercentage: {OverrideDamagePercentage}
                OverrideCooldownTime: {OverrideCooldownTime}
                OverrideMaxAmount: {OverrideMaxAmount}
                OverrideUnlocked: {OverrideUnlocked}
                UseOverrideUnlocked: {UseOverrideUnlocked}
            }}";
    }
}

