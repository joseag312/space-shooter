using Godot;

[GlobalClass]
public partial class WeaponDataComponent : Resource
{
    public const int LeftMuzzle = 0;
    public const int RightMuzzle = 1;
    public const int BothMuzzles = 2;
    public const int CenterCannon = 3;
    public const int Center = 4;

    public enum WeaponSlotType
    {
        Basic,
        Big,
        Slot
    }

    public enum WeaponType
    {
        Machine,
        Plasma,
        Ethereal,
        Chaos
    }

    [Export] public string Key { get; set; } = "";
    [Export] public string DisplayName { get; set; } = "";
    [Export] public string IconPath { get; set; } = "";
    [Export] public string ProjectilePath { get; set; }

    [Export(PropertyHint.Enum, "Left Muzzle,Right Muzzle,Both Muzzles,Center Cannon,Center")]
    public int SpawnLocation { get; set; } = LeftMuzzle;

    [Export(PropertyHint.Enum, "Basic,Big,Slot")]
    public WeaponSlotType SlotType { get; set; } = WeaponSlotType.Slot;

    [Export(PropertyHint.Enum, "Machine,Plasma,Ethereal,Chaos")]
    public WeaponType Type { get; set; } = WeaponType.Machine;

    // Overwriteable
    [Export] public int Damage { get; set; }
    [Export] public int DamagePercentage { get; set; }
    [Export] public float CooldownTime { get; set; } = 3.0f;
    [Export] public int MaxAmount { get; set; } = 10;
    [Export] public bool Unlocked { get; set; } = false;
}
