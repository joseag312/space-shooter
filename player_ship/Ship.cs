using Godot;

[GlobalClass]
public partial class Ship : Node2D
{
	[Export] public WeaponManagerComponent WeaponManager;
	[Export] public MoveComponent MoveComponent;
	[Export] public ScaleComponent ScaleComponent;
	[Export] public HurtboxComponent HurtboxComponent;
	[Export] public StatsComponent StatsComponent;
	[Export] public PositionClampComponent PositionClampComponent;
	[Export] public Node2D Anchor;

	[Signal] public delegate void HealthChangedEventHandler(int newHealth);

	private bool _readyToFire = false;

	public override void _Ready()
	{
		AnimateShipEntry(this);

		StatsComponent.MaxHealth = G.SS.Health;
		StatsComponent.Health = G.SS.Health;

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

		HurtboxComponent.Hurt += UpdateHealth;
	}

	public override void _Process(double delta)
	{
		if (_readyToFire)
		{
			WeaponManager?.SpawnWeapon(0);

			if (Input.IsActionJustPressed("fire_large"))
				WeaponManager?.SpawnWeapon(1);
			if (Input.IsActionJustPressed("fire_special_1"))
				WeaponManager?.SpawnWeapon(2);
			if (Input.IsActionJustPressed("fire_special_2"))
				WeaponManager?.SpawnWeapon(3);
			if (Input.IsActionJustPressed("fire_special_3"))
				WeaponManager?.SpawnWeapon(4);
			if (Input.IsActionJustPressed("fire_special_4"))
				WeaponManager?.SpawnWeapon(5);
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

	private void UpdateHealth(HitboxComponent hitboxComponent)
	{
		int damage = hitboxComponent.Damage;

		if (hitboxComponent.DamagePercentage > 0)
		{
			damage = (int)((hitboxComponent.DamagePercentage / 100.0f) * StatsComponent.MaxHealth);
			damage = Mathf.Max(damage, 1);
		}

		EmitSignal(nameof(HealthChanged), damage);
	}

	public void StartFiring() => _readyToFire = true;

	public void StopFiring() => _readyToFire = false;
}
