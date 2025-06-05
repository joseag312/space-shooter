using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

[GlobalClass]
public partial class WeaponManagerComponent : Node
{
	[Export] private NodePath leftMuzzlePath;
	[Export] private NodePath rightMuzzlePath;
	[Export] private NodePath centerCannonPath;
	[Export] private NodePath centerPath;

	private Marker2D center;
	private Marker2D leftMuzzle;
	private Marker2D rightMuzzle;
	private Marker2D centerCannon;

	private WeaponDataComponent basicWeapon;
	private WeaponDataComponent largeWeapon;
	private WeaponDataComponent[] specialWeapons;

	private Dictionary<int, float> weaponCooldowns = new();
	private Dictionary<int, float> currentCooldowns = new();

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		specialWeapons = new WeaponDataComponent[4];
		center = GetNode<Marker2D>(centerPath);
		leftMuzzle = GetNode<Marker2D>(leftMuzzlePath);
		rightMuzzle = GetNode<Marker2D>(rightMuzzlePath);
		centerCannon = GetNode<Marker2D>(centerCannonPath);
		AssignWeapons();
	}

	public override void _Process(double delta)
	{
		if (currentCooldowns.Count == 0)
		{
			GD.PrintErr("ERROR: WeaponManagerComponent - currentCooldowns is empty, cooldowns aren't being tracked");
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

	public void AssignWeapons()
	{
		basicWeapon = WeaponDatabase.Instance.BasicWeapon;
		largeWeapon = WeaponDatabase.Instance.LargeWeapon;
		specialWeapons = WeaponDatabase.Instance.SpecialWeapons;

		weaponCooldowns[0] = basicWeapon.CooldownTime;
		weaponCooldowns[1] = largeWeapon.CooldownTime;
		for (int i = 0; i < specialWeapons.Length; i++)
		{
			weaponCooldowns[i + 2] = specialWeapons[i].CooldownTime;
		}
		foreach (var key in weaponCooldowns.Keys)
		{
			currentCooldowns[key] = 0f;
		}
	}

	public void SpawnWeapon(int weaponSlot)
	{
		if (currentCooldowns.ContainsKey(weaponSlot) && currentCooldowns[weaponSlot] > 0)
		{
			return;
		}

		if (leftMuzzle == null || rightMuzzle == null || centerCannon == null)
		{
			GD.PrintErr("ERROR: WeaponManagerComponent - Muzzle points are not assigned");
		}

		WeaponDataComponent weaponData = weaponSlot == 0 ? basicWeapon :
								weaponSlot == 1 ? largeWeapon :
								specialWeapons[weaponSlot - 2];

		if (weaponData == null)
		{
			GD.PrintErr("ERROR: WeaponManagerComponent - Weapon slot " + weaponSlot + " is not assigned");
		}

		PackedScene weaponScene = ResourceLoader.Load<PackedScene>(weaponData.ProjectilePath);

		switch (weaponData.SpawnLocation)
		{
			case WeaponDataComponent.LeftMuzzle:
				SpawnAtPosition(weaponScene, leftMuzzle.GlobalPosition, WeaponDataComponent.LeftMuzzle);
				break;

			case WeaponDataComponent.RightMuzzle:
				SpawnAtPosition(weaponScene, rightMuzzle.GlobalPosition, WeaponDataComponent.RightMuzzle);
				break;

			case WeaponDataComponent.BothMuzzles:
				SpawnAtPosition(weaponScene, leftMuzzle.GlobalPosition, WeaponDataComponent.LeftMuzzle);
				SpawnAtPosition(weaponScene, rightMuzzle.GlobalPosition, WeaponDataComponent.RightMuzzle);
				break;

			case WeaponDataComponent.CenterCannon:
				SpawnAtPosition(weaponScene, centerCannon.GlobalPosition, WeaponDataComponent.CenterCannon);
				break;

			case WeaponDataComponent.Center:
				SpawnAtPosition(weaponScene, center.GlobalPosition, WeaponDataComponent.Center);
				break;

			default:
				GD.PrintErr("ERROR: WeaponManagerComponent - Unknown weapon spawn location in " + weaponData.SpawnLocation);
				return;
		}

		currentCooldowns[weaponSlot] = weaponCooldowns[weaponSlot];
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

		weaponInstance.AddToGroup("despawnable");
	}

}
