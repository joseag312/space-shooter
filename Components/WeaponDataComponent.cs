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

	[Export] public String projectilePath;
	[Export] public String projectileName;
	[Export] public int damage;
	[Export] public int damagePercentage;
	[Export] public float cooldownTime = 3.0f;
	[Export(PropertyHint.Enum, "Left Muzzle,Right Muzzle,Both Muzzles,Center Cannon,Center")]
	public int spawnLocation = LeftMuzzle;
}