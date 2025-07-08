using Godot;
using System;

[GlobalClass]
public partial class WeaponDataLoaderComponent : Node
{
	[Export] public string WeaponKey = "basic_blaster";
	[Export] public HitboxComponent hitboxComponent;

	public override void _Ready()
	{
		var state = G.WI.GetWeaponState(WeaponKey);
		if (state == null)
		{
			GD.PrintErr($"ERROR: WeaponDataLoaderComponent - No state found for '{WeaponKey}'");
			return;
		}

		if (state.BaseData == null)
		{
			state.BaseData = G.WD.GetWeaponData(WeaponKey);
			if (state.BaseData == null)
			{
				GD.PrintErr($"ERROR: WeaponDataLoaderComponent - No base data found for '{WeaponKey}'");
				return;
			}
		}

		hitboxComponent.Damage = state.EffectiveDamage;
		hitboxComponent.DamagePercentage = state.EffectiveDamagePercentage;
	}
}
