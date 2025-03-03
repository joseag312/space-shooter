using Godot;
using System.Diagnostics;

[GlobalClass]
public partial class Ship : Node2D
{
	[Export] private ShipWeaponManager weaponManager;
	[Export] private MoveComponent moveComponent;
	[Export] private Node2D anchor;

	public override void _Ready()
	{
		if (weaponManager != null)
		{
			if (WeaponDatabase.Instance.BasicWeapon == null)
			{
				GD.PrintErr("ERROR: Basic Weapon is NULL in WeaponDatabase!");
			}

			if (WeaponDatabase.Instance.LargeWeapon == null)
			{
				GD.PrintErr("ERROR: Large Weapon is NULL in WeaponDatabase!");
			}

			if (WeaponDatabase.Instance.SpecialWeapons[0] == null)
			{
				GD.PrintErr("ERROR: Special Weapon 1 is NULL in WeaponDatabase!");
			}

			if (WeaponDatabase.Instance.SpecialWeapons[1] == null)
			{
				GD.PrintErr("ERROR: Special Weapon 2 is NULL in WeaponDatabase!");
			}

			if (WeaponDatabase.Instance.SpecialWeapons[2] == null)
			{
				GD.PrintErr("ERROR: Special Weapon 3 is NULL in WeaponDatabase!");
			}

			if (WeaponDatabase.Instance.SpecialWeapons[3] == null)
			{
				GD.PrintErr("ERROR: Special Weapon 4 is NULL in WeaponDatabase!");
			}

			weaponManager.AssignWeapons(
				WeaponDatabase.Instance.BasicWeapon,
				WeaponDatabase.Instance.LargeWeapon,
				WeaponDatabase.Instance.SpecialWeapons
			);
		}
		else
		{
			GD.PrintErr("ERROR: WeaponManager is NOT assigned to Ship!");
		}
	}


	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("fire_basic"))
			weaponManager?.SpawnWeapon(0);

		if (Input.IsActionJustPressed("fire_large"))
			weaponManager?.SpawnWeapon(1);

		if (Input.IsActionJustPressed("fire_special_1"))
			weaponManager?.SpawnWeapon(2);

		if (Input.IsActionJustPressed("fire_special_2"))
			weaponManager?.SpawnWeapon(3);

		if (Input.IsActionJustPressed("fire_special_3"))
			weaponManager?.SpawnWeapon(4);

		if (Input.IsActionJustPressed("fire_special_4"))
			weaponManager?.SpawnWeapon(5);

		AnimateShip();
	}

	public void AnimateShip()
	{
		foreach (AnimatedSprite2D sprite in anchor.GetChildren())
		{
			if (moveComponent.Velocity.X < 0)
			{
				sprite.Play("BankLeft");
			}
			else if (moveComponent.Velocity.X > 0)
			{
				sprite.Play("BankRight");
			}
			else
			{
				sprite.Play("Center");
			}
		}
	}
}
