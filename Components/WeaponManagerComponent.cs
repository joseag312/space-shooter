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

	private WeaponDataComponent _basicWeapon;
	private WeaponDataComponent _largeWeapon;
	private WeaponDataComponent[] _specialWeapons;

	private Dictionary<int, float> _weaponCooldowns = new();
	private Dictionary<int, float> _currentCooldowns = new();

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		_specialWeapons = new WeaponDataComponent[4];
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
		_basicWeapon = G.WD.BasicWeapon;
		_largeWeapon = G.WD.LargeWeapon;
		_specialWeapons = G.WD.SpecialWeapons;

		_weaponCooldowns[0] = _basicWeapon.CooldownTime;
		_weaponCooldowns[1] = _largeWeapon.CooldownTime;
		for (int i = 0; i < _specialWeapons.Length; i++)
		{
			_weaponCooldowns[i + 2] = _specialWeapons[i].CooldownTime;
		}
		foreach (var key in _weaponCooldowns.Keys)
		{
			_currentCooldowns[key] = 0f;
		}
	}

	public void SpawnWeapon(int weaponSlot, Node2D targetContainer)
	{
		if (_currentCooldowns.ContainsKey(weaponSlot) && _currentCooldowns[weaponSlot] > 0)
		{
			return;
		}

		if (_leftMuzzle == null || _rightMuzzle == null || _centerCannon == null)
		{
			GD.PrintErr("ERROR: WeaponManagerComponent - Muzzle points are not assigned");
		}

		WeaponDataComponent weaponData = weaponSlot == 0 ? _basicWeapon :
								weaponSlot == 1 ? _largeWeapon :
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
