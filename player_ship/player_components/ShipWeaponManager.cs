using System.Collections.Generic;
using System.Diagnostics;
using Godot;

[GlobalClass]
public partial class ShipWeaponManager : Node
{
	[Export] private NodePath leftMuzzlePath;
	[Export] private NodePath rightMuzzlePath;
	[Export] private NodePath centerCannonPath;
	[Export] private NodePath centerPath;

	private Marker2D leftMuzzle;
	private Marker2D rightMuzzle;
	private Marker2D centerCannon;
	private Marker2D center;

	private PackedScene basicWeapon;
	private PackedScene largeWeapon;
	private PackedScene[] specialWeapons = new PackedScene[4];

	private Dictionary<int, float> weaponCooldowns = new();
	private Dictionary<int, float> currentCooldowns = new();

	public override void _Ready()
	{
		leftMuzzle = GetNode<Marker2D>(leftMuzzlePath);
		rightMuzzle = GetNode<Marker2D>(rightMuzzlePath);
		centerCannon = GetNode<Marker2D>(centerCannonPath);
		center = GetNode<Marker2D>(centerPath);
		ProcessMode = ProcessModeEnum.Always;
	}

	public override void _Process(double delta)
	{
		if (currentCooldowns.Count == 0)
		{
			GD.PrintErr("ERROR: ShipWeaponManager - currentCooldowns is empty, cooldowns aren't being tracked");
			return;
		}

		foreach (var key in currentCooldowns.Keys)
		{
			if (currentCooldowns[key] > 0f)
			{
				currentCooldowns[key] -= (float)delta;
				if (currentCooldowns[key] < 0f)
				{
					currentCooldowns[key] = 0f;
				}
			}
		}
	}

	public void AssignWeapons(PackedScene basic, PackedScene large, PackedScene[] specials)
	{
		GD.Print("DEBUG: ShipWeaponManager - AssignWeapons() is being called...");
		if (basic == null)
		{
			GD.PrintErr("ERROR: ShipWeaponManager - Basic weapon is NULL when assigned");
		}

		if (large == null)
		{
			GD.PrintErr("ERROR: ShipWeaponManager - Large weapon is NULL when assigned");
		}

		if (specials == null)
		{
			GD.PrintErr("ERROR: ShipWeaponManager - Special weapons array is NULL when assigned");
		}

		basicWeapon = basic;
		largeWeapon = large;
		specialWeapons = specials;

		weaponCooldowns[0] = GetWeaponCooldown(basicWeapon);
		weaponCooldowns[1] = GetWeaponCooldown(largeWeapon);
		for (int i = 0; i < specialWeapons.Length; i++)
		{
			weaponCooldowns[i + 2] = GetWeaponCooldown(specialWeapons[i]);
		}
		foreach (var key in weaponCooldowns.Keys)
		{
			currentCooldowns[key] = 0f;
		}
		foreach (var key in weaponCooldowns.Keys)
		{
			GD.Print($"DEBUG: ShipWeaponManager - Weapon slot {key} has cooldown: {weaponCooldowns[key]}");
		}
		GD.Print($"DEBUG: ShipWeaponManager - currentCooldowns initialized with {currentCooldowns.Count} entries");

	}

	private float GetWeaponCooldown(PackedScene weaponScene)
	{
		if (weaponScene == null)
		{
			GD.PrintErr("ERROR: ShipWeaponManager - Trying to get cooldown from NULL weapon");
			return 0.5f;
		}

		Node2D tempWeapon = weaponScene.Instantiate<Node2D>();
		WeaponData weaponData = tempWeapon.GetNodeOrNull<WeaponData>("WeaponData");

		if (weaponData == null)
		{
			GD.PrintErr($"ERROR: ShipWeaponManager - Weapon scene {weaponScene.ResourcePath} has no WeaponData component");
			return 0.5f;
		}

		float cooldown = weaponData.CooldownTime;
		tempWeapon.QueueFree();
		return cooldown;
	}

	public void SpawnWeapon(int weaponSlot)
	{
		if (currentCooldowns.ContainsKey(weaponSlot) && currentCooldowns[weaponSlot] > 0)
		{
			return;
		}

		if (leftMuzzle == null || rightMuzzle == null || centerCannon == null)
		{
			GD.PrintErr("ERROR: ShipWeaponManager - Muzzle points are not assigned");
		}


		PackedScene weaponToSpawn = weaponSlot == 0 ? basicWeapon :
									weaponSlot == 1 ? largeWeapon :
									specialWeapons[weaponSlot - 2];

		if (weaponToSpawn == null)
		{
			GD.PrintErr("ERROR: ShipWeaponManager - Weapon slot " + weaponSlot + " is not assigned");
		}

		if (weaponToSpawn != null)
		{
			Node2D weaponInstance = (Node2D)weaponToSpawn.Instantiate();
			WeaponData weaponData = weaponInstance.GetNodeOrNull<WeaponData>("WeaponData");

			if (weaponData != null)
			{
				switch (weaponData.spawnLocation)
				{
					case WeaponData.LeftMuzzle:
						SpawnAtPosition(weaponToSpawn, leftMuzzle.GlobalPosition, WeaponData.LeftMuzzle);
						break;

					case WeaponData.RightMuzzle:
						SpawnAtPosition(weaponToSpawn, rightMuzzle.GlobalPosition, WeaponData.RightMuzzle);
						break;

					case WeaponData.BothMuzzles:
						SpawnAtPosition(weaponToSpawn, leftMuzzle.GlobalPosition, WeaponData.LeftMuzzle);
						SpawnAtPosition(weaponToSpawn, rightMuzzle.GlobalPosition, WeaponData.RightMuzzle);
						break;

					case WeaponData.CenterCannon:
						SpawnAtPosition(weaponToSpawn, centerCannon.GlobalPosition, WeaponData.CenterCannon);
						break;

					case WeaponData.Center:
						SpawnAtPosition(weaponToSpawn, center.GlobalPosition, WeaponData.Center);
						break;

					default:
						GD.PrintErr("ERROR: ShipWeaponManager - Unknown weapon spawn location in " + weaponData.spawnLocation);
						return;
				}

				currentCooldowns[weaponSlot] = weaponCooldowns[weaponSlot];
			}
			else
			{
				GD.PrintErr("ERROR: ShipWeaponManager - WeaponData component is missing in " + weaponInstance.Name);
			}
		}
	}

	private void SpawnAtPosition(PackedScene weaponScene, Vector2 position, int locationType)
	{
		Node2D weaponInstance = (Node2D)weaponScene.Instantiate();

		if (locationType == 4)
		{
			Node2D ship = GetParent<Node2D>();
			ship.AddChild(weaponInstance);
		}
		else
		{
			weaponInstance.GlobalPosition = position;
			GetTree().CurrentScene.AddChild(weaponInstance);
		}
	}

}
