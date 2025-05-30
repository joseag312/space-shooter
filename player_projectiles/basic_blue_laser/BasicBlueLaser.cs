using Godot;

public partial class BasicBlueLaser : Node2D
{
	[Export] private ScaleComponent scaleComponent;
	[Export] private FlashComponent flashComponent;
	[Export] private HitboxComponent hitboxComponent;

	public override void _Ready()
	{
		flashComponent.Flash();
		scaleComponent.TweenScale();
		hitboxComponent.HitHurtbox += OnLaserHit;
		AddToGroup("despawnable");
	}

	private void OnLaserHit(HurtboxComponent hurtbox)
	{
		QueueFree();
	}
}
