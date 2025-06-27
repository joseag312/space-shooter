using Godot;

[GlobalClass]
public partial class Ship : Node2D
{
	[Export] public Node2D ProjectileContainer;
	[Export] public WeaponManagerComponent WeaponManager;
	[Export] public MoveComponent MoveComponent;
	[Export] public ScaleComponent ScaleComponent;
	[Export] public HurtboxComponent HurtboxComponent;
	[Export] public StatsComponent StatsComponent;
	[Export] public PositionClampComponent PositionClampComponent;
	[Export] public Node2D Anchor;

	[Signal]
	public delegate void HealthChangedEventHandler(int oldHealth, int newHealth);
	[Signal] public delegate void ShipReadyEventHandler();

	private bool _readyToFire = false;

	public override void _Ready()
	{
		AnimateShipEntry(this);

		StatsComponent.MaxHealth = G.SS.Health;
		StatsComponent.Health = G.SS.Health;
		EmitSignal(nameof(ShipReady));
		EmitSignal(nameof(HealthChanged), StatsComponent.Health, StatsComponent.Health);

		if (WeaponManager != null)
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

			WeaponManager.AssignWeapons();
		}
		else
		{
			GD.PrintErr("ERROR: Ship - WeaponManager is NOT assigned to Ship");
		}
	}

	public override void _Process(double delta)
	{
		if (_readyToFire && !G.GF.IsInputBlocked)
		{
			WeaponManager?.SpawnWeapon(0, ProjectileContainer);

			if (Input.IsActionJustPressed("fire_large"))
				WeaponManager?.SpawnWeapon(1, ProjectileContainer);
			if (Input.IsActionJustPressed("fire_special_1"))
				WeaponManager?.SpawnWeapon(2, ProjectileContainer);
			if (Input.IsActionJustPressed("fire_special_2"))
				WeaponManager?.SpawnWeapon(3, ProjectileContainer);
			if (Input.IsActionJustPressed("fire_special_3"))
				WeaponManager?.SpawnWeapon(4, ProjectileContainer);
			if (Input.IsActionJustPressed("fire_special_4"))
				WeaponManager?.SpawnWeapon(5, ProjectileContainer);
		}

		AnimateShip();
	}

	public async void AnimateShipEntry(Node2D ship)
	{
		PositionClampComponent.Enabled = false;
		ship.Position = new Vector2(640, 1080);

		var tween = GetTree().CreateTween();
		tween.TweenProperty(ship, "position", new Vector2(640, 600), 1.0f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.Out);

		await ToSignal(tween, "finished");

		PositionClampComponent.Enabled = true;
		StartFiring();
	}

	public void AnimateShip()
	{
		foreach (AnimatedSprite2D sprite in Anchor.GetChildren())
		{
			if (MoveComponent.Velocity.X < 0)
			{
				RotationDegrees = -5;
				Skew = Mathf.DegToRad(5);
				sprite.Play("BankLeft");
			}
			else if (MoveComponent.Velocity.X > 0)
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

	public void StartFiring() => _readyToFire = true;

	public void StopFiring() => _readyToFire = false;

	public void Heal(int healPercent)
	{
		int oldHealth = StatsComponent.Health;
		int healAmount = Mathf.Max((int)((healPercent / 100.0f) * StatsComponent.MaxHealth), 1);
		int newHealth = Mathf.Min(oldHealth + healAmount, StatsComponent.MaxHealth);

		if (newHealth != oldHealth)
		{
			StatsComponent.Health = newHealth;
			EmitSignal(nameof(HealthChanged), oldHealth, newHealth);
			SpawnHealText(newHealth - oldHealth);
		}
		else
		{
			GD.Print("DEBUG: Ship - Heal skipped: Already at max health");
		}
	}

	private void SpawnHealText(int healAmount)
	{
		PackedScene healTextScene = ResourceLoader.Load<PackedScene>("res://player_ship/ship_heal_text.tscn");

		if (healTextScene == null)
		{
			GD.PrintErr("ERROR: Ship - ship_heal_text.tscn could not be loaded!");
			return;
		}

		var healText = healTextScene.Instantiate<ShipHealText>();
		healText.Initialize(healAmount);
		AddChild(healText);
	}
}
