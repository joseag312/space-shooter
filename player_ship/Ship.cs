using Godot;

[GlobalClass]
public partial class Ship : Node2D
{
	[Export] private ShipWeaponManager weaponManager;
	[Export] private MoveComponent moveComponent;
	[Export] private ScaleComponent scaleComponent;
	[Export] private HurtboxComponent hurtboxComponent;
	[Export] private Node2D anchor;
	[Signal] public delegate void HealthChangedEventHandler(int newHealth);

	public override void _Ready()
	{
		if (weaponManager != null)
		{
			if (WeaponDatabase.Instance.BasicWeapon == null)
			{
				GD.PrintErr("ERROR: Ship - Basic Weapon is NULL in WeaponDatabase");
			}

			if (WeaponDatabase.Instance.LargeWeapon == null)
			{
				GD.PrintErr("ERROR: Ship - Large Weapon is NULL in WeaponDatabase");
			}

			if (WeaponDatabase.Instance.SpecialWeapons[0] == null)
			{
				GD.PrintErr("ERROR: Ship - Special Weapon 1 is NULL in WeaponDatabase");
			}

			if (WeaponDatabase.Instance.SpecialWeapons[1] == null)
			{
				GD.PrintErr("ERROR: Ship - Special Weapon 2 is NULL in WeaponDatabase");
			}

			if (WeaponDatabase.Instance.SpecialWeapons[2] == null)
			{
				GD.PrintErr("ERROR: Ship - Special Weapon 3 is NULL in WeaponDatabase");
			}

			if (WeaponDatabase.Instance.SpecialWeapons[3] == null)
			{
				GD.PrintErr("ERROR: Ship - Special Weapon 4 is NULL in WeaponDatabase");
			}

			weaponManager.AssignWeapons(
				WeaponDatabase.Instance.BasicWeapon,
				WeaponDatabase.Instance.LargeWeapon,
				WeaponDatabase.Instance.SpecialWeapons
			);
		}
		else
		{
			GD.PrintErr("ERROR: Ship - WeaponManager is NOT assigned to Ship");
		}

		hurtboxComponent.Hurt += OnHurt;
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
				RotationDegrees = -5;
				Skew = Mathf.DegToRad(5);
				sprite.Play("BankLeft");
			}
			else if (moveComponent.Velocity.X > 0)
			{
				Skew = Mathf.DegToRad(-5);
				RotationDegrees = 5;
				sprite.Play("BankRight");
			}
			else
			{
				Skew = 0;
				RotationDegrees = 0;
				sprite.Play("Center");
			}
		}
	}

	private void OnHurt(HitboxComponent hitboxComponent)
	{
		scaleComponent.TweenScale();
		int damageInt = (int)hitboxComponent.Damage;
		SpawnDamageText(damageInt);
		float damageFloat = hitboxComponent.Damage;
		EmitSignal(nameof(HealthChanged), damageFloat);
	}

	private void SpawnDamageText(int damage)
	{
		PackedScene damageTextScene = (PackedScene)ResourceLoader.Load("res://player_ship/ship_damage_text.tscn");

		if (damageTextScene == null)
		{
			GD.PrintErr("ERROR: Ship - ship_damage_text.tscn could not be loaded!");
			return;
		}

		ShipDamageText damageText = damageTextScene.Instantiate<ShipDamageText>();

		if (damageText == null)
		{
			GD.PrintErr("ERROR: Ship - Failed to instantiate DamageText!");
			return;
		}

		damageText.GlobalPosition = GlobalPosition + new Vector2(0, -10);
		damageText.Initialize(damage);
		GetParent().AddChild(damageText);
	}
}
