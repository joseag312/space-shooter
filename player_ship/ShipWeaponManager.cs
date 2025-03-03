using System.Collections.Generic;
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

	private Dictionary<int, float> weaponCooldowns = new();
	private Dictionary<int, float> currentCooldowns = new();

	public override void _Ready()
	{
		leftMuzzle = GetNode<Marker2D>(leftMuzzlePath);
		rightMuzzle = GetNode<Marker2D>(rightMuzzlePath);
		centerCannon = GetNode<Marker2D>(centerCannonPath);
		ProcessMode = ProcessModeEnum.Always;
	}

	public override void _Process(double delta)
	{
		if (currentCooldowns.Count == 0)
		{
			GD.PrintErr("ERROR: currentCooldowns is EMPTY! Cooldowns aren't being tracked.");
			return;
		}

		foreach (var key in currentCooldowns.Keys)
		{
			// GD.Print($"DEBUG: Weapon {key} cooldown remaining: {currentCooldowns[key]:F2} seconds");
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
		GD.Print("DEBUG: AssignWeapons() is being called!");
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
			GD.Print($"DEBUG: Weapon slot {key} has cooldown: {weaponCooldowns[key]}");
		}
		GD.Print($"DEBUG: currentCooldowns initialized with {currentCooldowns.Count} entries.");

	}

	private float GetWeaponCooldown(PackedScene weaponScene)
	{
		if (weaponScene == null)
		{
			GD.PrintErr("ERROR: Trying to get cooldown from NULL weapon!");
			return 0.5f;
		}

		Node2D tempWeapon = weaponScene.Instantiate<Node2D>();
		WeaponData weaponData = tempWeapon.GetNodeOrNull<WeaponData>("WeaponData");

		if (weaponData == null)
		{
			GD.PrintErr($"ERROR: Weapon scene {weaponScene.ResourcePath} has no WeaponData component!");
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
				switch (weaponData.spawnLocation)
				{
					case WeaponData.LeftMuzzle:
						SpawnAtPosition(weaponToSpawn, leftMuzzle.GlobalPosition);
						break;

					case WeaponData.RightMuzzle:
						SpawnAtPosition(weaponToSpawn, rightMuzzle.GlobalPosition);
						break;

					case WeaponData.BothMuzzles:
						SpawnAtPosition(weaponToSpawn, leftMuzzle.GlobalPosition);
						SpawnAtPosition(weaponToSpawn, rightMuzzle.GlobalPosition);
						break;

					case WeaponData.CenterCannon:
						SpawnAtPosition(weaponToSpawn, centerCannon.GlobalPosition);
						break;

					default:
						GD.PrintErr("ERROR: Unknown weapon spawn location: " + weaponData.spawnLocation);
						return;
				}

				currentCooldowns[weaponSlot] = weaponCooldowns[weaponSlot];
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
