using System;
using Godot;

[GlobalClass]
public partial class WeaponDataComponent : Node
{
	public const int LeftMuzzle = 0;
	public const int RightMuzzle = 1;
	public const int BothMuzzles = 2;
	public const int CenterCannon = 3;
	public const int Center = 4;

	[Export] public string ProjectilePath { get; set; }
	[Export] public string ProjectileName { get; set; }
	[Export] public int Damage { get; set; }
	[Export] public int DamagePercentage { get; set; }
	[Export] public float CooldownTime { get; set; } = 3.0f;

	[Export(PropertyHint.Enum, "Left Muzzle,Right Muzzle,Both Muzzles,Center Cannon,Center")]
	public int SpawnLocation { get; set; } = LeftMuzzle;
}
