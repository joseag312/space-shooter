using Godot;

public partial class Enemy : Node2D
{
	[Export] private StatsComponent statsComponent;
	[Export] private MoveComponent moveComponent;
	[Export] private ScaleComponent scaleComponent;
	[Export] private FlashComponent flashComponent;
	[Export] private ShakeComponent shakeComponent;
	[Export] private VisibleOnScreenNotifier2D visibleOnScreenNotifier2D;
	[Export] private HurtboxComponent hurtboxComponent;
	[Export] private HitboxComponent hitboxComponent;
	[Export] private TextureProgressBar healthBar;

	public override void _Ready()
	{
		healthBar.MaxValue = statsComponent.Health;
		healthBar.Value = statsComponent.Health;
		hurtboxComponent.Hurt += OnHurt;
		statsComponent.NoHealth += IsDead;
	}

	private void UpdateHealthBar()
	{
		GD.Print("HealthBar should be updated");
		healthBar.Value = statsComponent.Health;
	}

	private void OnHurt(HitboxComponent hitboxComponent)
	{
		scaleComponent.TweenScale();
		flashComponent.Flash();
		shakeComponent.TweenShake();
		UpdateHealthBar();
	}

	private void IsDead()
	{
		QueueFree();
	}

	public override void _ExitTree()
	{
		hurtboxComponent.Hurt -= OnHurt;
		statsComponent.NoHealth -= IsDead;
	}
}
