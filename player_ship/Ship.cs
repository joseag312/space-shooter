using Godot;

[GlobalClass]
public partial class Ship : Node2D
{
	[Export] private WeaponManagerComponent _weaponManager;
	[Export] private MoveComponent _moveComponent;
	[Export] private ScaleComponent _scaleComponent;
	[Export] private HurtboxComponent _hurtboxComponent;
	[Export] private StatsComponent _statsComponent;
	[Export] private PositionClampComponent _positionClampComponent;
	[Export] private Node2D _anchor;

	[Signal] public delegate void HealthChangedEventHandler(int newHealth);

	private bool _readyToFire = false;

	public override void _Ready()
	{
		AnimateShipEntry(this);

		_statsComponent.MaxHealth = G.SS.Health;
		_statsComponent.Health = G.SS.Health;

		if (_weaponManager != null)
		{
			if (G.WD.BasicWeapon == null)
				GD.PrintErr("ERROR: Ship - Basic Weapon is NULL in AutoWeaponDatabase");
			if (G.WD.LargeWeapon == null)
				GD.PrintErr("ERROR: Ship - Large Weapon is NULL in AutoWeaponDatabase");
			if (G.WD.SpecialWeapons[0] == null)
				GD.PrintErr("ERROR: Ship - Special Weapon 1 is NULL in AutoWeaponDatabase");
			if (G.WD.SpecialWeapons[1] == null)
				GD.PrintErr("ERROR: Ship - Special Weapon 2 is NULL in AutoWeaponDatabase");
			if (G.WD.SpecialWeapons[2] == null)
				GD.PrintErr("ERROR: Ship - Special Weapon 3 is NULL in AutoWeaponDatabase");
			if (G.WD.SpecialWeapons[3] == null)
				GD.PrintErr("ERROR: Ship - Special Weapon 4 is NULL in AutoWeaponDatabase");

			_weaponManager.AssignWeapons();
		}
		else
		{
			GD.PrintErr("ERROR: Ship - WeaponManager is NOT assigned to Ship");
		}

		_hurtboxComponent.Hurt += UpdateHealth;
	}

	public override void _Process(double delta)
	{
		if (_readyToFire)
		{
			_weaponManager?.SpawnWeapon(0);

			if (Input.IsActionJustPressed("fire_large"))
				_weaponManager?.SpawnWeapon(1);
			if (Input.IsActionJustPressed("fire_special_1"))
				_weaponManager?.SpawnWeapon(2);
			if (Input.IsActionJustPressed("fire_special_2"))
				_weaponManager?.SpawnWeapon(3);
			if (Input.IsActionJustPressed("fire_special_3"))
				_weaponManager?.SpawnWeapon(4);
			if (Input.IsActionJustPressed("fire_special_4"))
				_weaponManager?.SpawnWeapon(5);
		}

		AnimateShip();
	}

	public void AnimateShip()
	{
		foreach (AnimatedSprite2D sprite in _anchor.GetChildren())
		{
			if (_moveComponent.Velocity.X < 0)
			{
				RotationDegrees = -5;
				Skew = Mathf.DegToRad(5);
				sprite.Play("BankLeft");
			}
			else if (_moveComponent.Velocity.X > 0)
			{
				RotationDegrees = 5;
				Skew = Mathf.DegToRad(-5);
				sprite.Play("BankRight");
			}
			else
			{
				RotationDegrees = 0;
				Skew = 0;
				sprite.Play("Center");
			}
		}
	}

	private void UpdateHealth(HitboxComponent hitboxComponent)
	{
		int damage = hitboxComponent.Damage;

		if (hitboxComponent.DamagePercentage > 0)
		{
			damage = (int)((hitboxComponent.DamagePercentage / 100.0f) * _statsComponent.MaxHealth);
			damage = Mathf.Max(damage, 1);
		}

		EmitSignal(nameof(HealthChanged), damage);
	}

	public async void AnimateShipEntry(Node2D ship)
	{
		_positionClampComponent.Enabled = false;
		ship.Position = new Vector2(640, 1080);

		var tween = GetTree().CreateTween();
		tween.TweenProperty(ship, "position", new Vector2(640, 600), 1.0f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.Out);

		await ToSignal(tween, "finished");

		_positionClampComponent.Enabled = true;
		StartFiring();
	}

	public void StartFiring() => _readyToFire = true;

	public void StopFiring() => _readyToFire = false;
}
