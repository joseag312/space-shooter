using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class WeaponManagerComponent : Node
{
	[Export] private NodePath _leftMuzzlePath;
	[Export] private NodePath _rightMuzzlePath;
	[Export] private NodePath _centerCannonPath;
	[Export] private NodePath _centerPath;

	private Marker2D _center;
	private Marker2D _leftMuzzle;
	private Marker2D _rightMuzzle;
	private Marker2D _centerCannon;

	private EffectiveWeaponData _basicWeapon;
	private EffectiveWeaponData _bigWeapon;
	private EffectiveWeaponData[] _specialWeapons = new EffectiveWeaponData[4];

	private Dictionary<string, float> _weaponCooldowns = new();
	private Dictionary<string, float> _currentCooldowns = new();

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		_center = GetNode<Marker2D>(_centerPath);
		_leftMuzzle = GetNode<Marker2D>(_leftMuzzlePath);
		_rightMuzzle = GetNode<Marker2D>(_rightMuzzlePath);
		_centerCannon = GetNode<Marker2D>(_centerCannonPath);
		AssignWeapons();
	}

	public override void _Process(double delta)
	{
		if (_currentCooldowns.Count == 0)
		{
			GD.PrintErr("ERROR: WeaponManagerComponent - currentCooldowns is empty, cooldowns aren't being tracked");
			return;
		}

		foreach (var key in _currentCooldowns.Keys)
		{
			if (_currentCooldowns[key] > 0f)
			{
				_currentCooldowns[key] -= (float)delta;
				if (_currentCooldowns[key] < 0f)
				{
					_currentCooldowns[key] = 0f;
				}
			}
		}
	}

	public void AssignWeapons()
	{
		var inventory = G.WI;
		string basicKey = inventory.GetEquippedWeaponKey(WeaponSlotNames.Basic);
		string bigKey = inventory.GetEquippedWeaponKey(WeaponSlotNames.Big);
		string[] slotKeys =
		{
			inventory.GetEquippedWeaponKey(WeaponSlotNames.Special1),
			inventory.GetEquippedWeaponKey(WeaponSlotNames.Special2),
			inventory.GetEquippedWeaponKey(WeaponSlotNames.Special3),
			inventory.GetEquippedWeaponKey(WeaponSlotNames.Special4)
		};

		_basicWeapon = inventory.GetEffectiveWeaponData(basicKey);
		_bigWeapon = inventory.GetEffectiveWeaponData(bigKey);
		for (int i = 0; i < 4; i++)
		{
			_specialWeapons[i] = inventory.GetEffectiveWeaponData(slotKeys[i]);
		}

		_weaponCooldowns.Clear();
		_currentCooldowns.Clear();

		if (_basicWeapon != null)
			_weaponCooldowns[basicKey] = _basicWeapon.CooldownTime;

		if (_bigWeapon != null)
			_weaponCooldowns[bigKey] = _bigWeapon.CooldownTime;

		if (_specialWeapons[0] != null)
			_weaponCooldowns[slotKeys[0]] = _specialWeapons[0].CooldownTime;

		if (_specialWeapons[1] != null)
			_weaponCooldowns[slotKeys[1]] = _specialWeapons[1].CooldownTime;

		if (_specialWeapons[2] != null)
			_weaponCooldowns[slotKeys[2]] = _specialWeapons[2].CooldownTime;

		if (_specialWeapons[3] != null)
			_weaponCooldowns[slotKeys[3]] = _specialWeapons[3].CooldownTime;

		foreach (var key in _weaponCooldowns.Keys)
		{
			_currentCooldowns[key] = 0f;
		}
	}

	public void SpawnWeapon(int weaponSlot, Node2D targetContainer)
	{

		G.WI.GetEquippedWeaponKey();

		if (_currentCooldowns.ContainsKey(weaponSlot) && _currentCooldowns[weaponSlot] > 0)
		{
			return;
		}

		if (_leftMuzzle == null || _rightMuzzle == null || _centerCannon == null)
		{
			GD.PrintErr("ERROR: WeaponManagerComponent - Muzzle points are not assigned");
		}

		EffectiveWeaponData weaponData = weaponSlot == 0 ? _basicWeapon :
								weaponSlot == 1 ? _bigWeapon :
								_specialWeapons[weaponSlot - 2];

		if (weaponData == null)
		{
			GD.PrintErr("ERROR: WeaponManagerComponent - Weapon slot " + weaponSlot + " is not assigned");
		}

		PackedScene weaponScene = ResourceLoader.Load<PackedScene>(weaponData.ProjectilePath);

		switch (weaponData.SpawnLocation)
		{
			case WeaponDataComponent.LeftMuzzle:
				SpawnAtPosition(weaponScene, _leftMuzzle.GlobalPosition, WeaponDataComponent.LeftMuzzle, targetContainer);
				break;

			case WeaponDataComponent.RightMuzzle:
				SpawnAtPosition(weaponScene, _rightMuzzle.GlobalPosition, WeaponDataComponent.RightMuzzle, targetContainer);
				break;

			case WeaponDataComponent.BothMuzzles:
				SpawnAtPosition(weaponScene, _leftMuzzle.GlobalPosition, WeaponDataComponent.LeftMuzzle, targetContainer);
				SpawnAtPosition(weaponScene, _rightMuzzle.GlobalPosition, WeaponDataComponent.RightMuzzle, targetContainer);
				break;

			case WeaponDataComponent.CenterCannon:
				SpawnAtPosition(weaponScene, _centerCannon.GlobalPosition, WeaponDataComponent.CenterCannon, targetContainer);
				break;

			case WeaponDataComponent.Center:
				SpawnAtPosition(weaponScene, _center.GlobalPosition, WeaponDataComponent.Center, targetContainer);
				break;

			default:
				GD.PrintErr("ERROR: WeaponManagerComponent - Unknown weapon spawn location in " + weaponData.SpawnLocation);
				return;
		}

		_currentCooldowns[weaponSlot] = _weaponCooldowns[weaponSlot];
	}

	private void SpawnAtPosition(PackedScene weaponScene, Vector2 position, int locationType, Node2D targetContainer)
	{
		Node2D weaponInstance = (Node2D)weaponScene.Instantiate();
		weaponInstance.AddToGroup("despawnable");

		if (locationType == 4)
		{
			Node2D ship = GetParent<Node2D>();
			ship.AddChild(weaponInstance);
			weaponInstance.Position = ship.ToLocal(position);
		}
		else
		{
			if (targetContainer == null)
			{
				GD.PrintErr("ERROR: WeaponManagerComponent - Target container is null. Cannot add projectile.");
				return;
			}

			weaponInstance.GlobalPosition = position;
			targetContainer.AddChild(weaponInstance);
		}
	}
}
