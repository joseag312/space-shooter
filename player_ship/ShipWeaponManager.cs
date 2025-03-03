using System.Diagnostics;
using Godot;

[GlobalClass]
public partial class ShipWeaponManager : Node
{
	[Export] private NodePath leftMuzzlePath;
	[Export] private NodePath rightMuzzlePath;
	[Export] private NodePath centerCannonPath;

	private Marker2D leftMuzzle;
	private Marker2D rightMuzzle;
	private Marker2D centerCannon;

	private PackedScene basicWeapon;
	private PackedScene largeWeapon;
	private PackedScene[] specialWeapons = new PackedScene[4];

	public override void _Ready()
	{
		leftMuzzle = GetNode<Marker2D>(leftMuzzlePath);
		rightMuzzle = GetNode<Marker2D>(rightMuzzlePath);
		centerCannon = GetNode<Marker2D>(centerCannonPath);
	}

	public void AssignWeapons(PackedScene basic, PackedScene large, PackedScene[] specials)
	{
		if (basic == null)
		{
			GD.PrintErr("ERROR: Basic weapon is NULL when assigned!");
		}

		if (large == null)
		{
			GD.PrintErr("ERROR: Large weapon is NULL when assigned!");
		}

		if (specials == null)
		{
			GD.PrintErr("ERROR: Special weapons array is NULL when assigned!");
		}

		basicWeapon = basic;
		largeWeapon = large;
		specialWeapons = specials;
	}


	public void SpawnWeapon(int weaponSlot)
	{
		if (leftMuzzle == null || rightMuzzle == null || centerCannon == null)
		{
			GD.PrintErr("ERROR: Muzzle points are not assigned in ShipWeaponManager!");
		}


		PackedScene weaponToSpawn = weaponSlot == 0 ? basicWeapon :
									weaponSlot == 1 ? largeWeapon :
									specialWeapons[weaponSlot - 2];

		if (weaponToSpawn == null)
		{
			GD.PrintErr("ERROR: Weapon slot " + weaponSlot + " is not assigned!");
		}

		if (weaponToSpawn != null)
		{
			Node2D weaponInstance = (Node2D)weaponToSpawn.Instantiate();
			WeaponData weaponData = weaponInstance.GetNodeOrNull<WeaponData>("WeaponData");

			if (weaponData != null)
			{
				// Get the correct spawn position
				Vector2 spawnPosition = Vector2.Zero;

				switch (weaponData.spawnLocation)
				{
					case WeaponData.LeftMuzzle:
						spawnPosition = leftMuzzle.GlobalPosition;
						break;

					case WeaponData.RightMuzzle:
						spawnPosition = rightMuzzle.GlobalPosition;
						break;

					case WeaponData.BothMuzzles:
						SpawnAtPosition(weaponToSpawn, leftMuzzle.GlobalPosition);
						SpawnAtPosition(weaponToSpawn, rightMuzzle.GlobalPosition);
						return;

					case WeaponData.CenterCannon:
						spawnPosition = centerCannon.GlobalPosition;
						break;

					default:
						GD.PrintErr("ERROR: Unknown weapon spawn location: " + weaponData.spawnLocation);
						return;
				}

				// Spawn weapon at correct position
				SpawnAtPosition(weaponToSpawn, spawnPosition);
			}
			else
			{
				GD.PrintErr("ERROR: WeaponData component is missing in " + weaponInstance.Name);
			}
		}
	}
	private void SpawnAtPosition(PackedScene weaponScene, Vector2 position)
	{
		Node2D weaponInstance = (Node2D)weaponScene.Instantiate();
		weaponInstance.GlobalPosition = position;

		GetTree().CurrentScene.AddChild(weaponInstance);
	}

}
