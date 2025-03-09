using Godot;
using System;

[GlobalClass]
public partial class WeaponDataLoaderComponent : Node
{
	[Export] public WeaponDataComponent weaponData;
	[Export] public HitboxComponent hitboxComponent;
	public override void _Ready()
	{
		WeaponDataComponent data = WeaponDatabase.Instance.GetWeaponData(weaponData.projectileName);
		weaponData.damage = data.damage;
		weaponData.damagePercentage = data.damagePercentage;
		weaponData.cooldownTime = data.cooldownTime;
		weaponData.spawnLocation = data.spawnLocation;
		hitboxComponent.Damage = data.damage;
		hitboxComponent.DamagePercentage = data.damagePercentage;
	}
}
