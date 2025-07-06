using Godot;
using System;

[GlobalClass]
public partial class WeaponDataLoaderComponent : Node
{
	[Export] public string WeaponKey = "basic_blaster";
	[Export] public HitboxComponent hitboxComponent;

	public override void _Ready()
	{
		var baseData = G.WD.GetWeaponData(WeaponKey);
		var instance = G.WI.GetInstanceData(WeaponKey);

		if (baseData == null)
		{
			GD.PrintErr($"ERROR: No base weapon data found for '{WeaponKey}'");
			return;
		}
		if (instance == null)
		{
			GD.PrintErr($"ERROR: No weapon instance data found for '{WeaponKey}'");
			return;
		}

		int finalDamage = instance.GetEffectiveDamage(baseData);
		int finalPct = instance.GetEffectiveDamagePercentage(baseData);

		hitboxComponent.Damage = finalDamage;
		hitboxComponent.DamagePercentage = finalPct;

	}
}

