using Godot;
using System;

[GlobalClass]
public partial class WeaponDataLoaderComponent : Node
{
	[Export] public WeaponDataComponent weaponData;
	[Export] public HitboxComponent hitboxComponent;
	public override void _Ready()
	{
		WeaponDataComponent data = G.WD.GetWeaponData(weaponData.ProjectileName);
		weaponData.Damage = data.Damage;
		weaponData.DamagePercentage = data.DamagePercentage;
		weaponData.CooldownTime = data.CooldownTime;
		weaponData.SpawnLocation = data.SpawnLocation;
		hitboxComponent.Damage = data.Damage;
		hitboxComponent.DamagePercentage = data.DamagePercentage;
	}
}
