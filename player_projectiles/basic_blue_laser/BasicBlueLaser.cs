using Godot;

public partial class BasicBlueLaser : Node2D
{
	[Export] private ScaleComponent _scaleComponent;
	[Export] private FlashComponent _flashComponent;
	[Export] private HitboxComponent _hitboxComponent;

	public override void _Ready()
	{
		_flashComponent.Flash();
		_scaleComponent.TweenScale();
		_hitboxComponent.HitHurtbox += OnLaserHit;
		AddToGroup("despawnable");
	}

	private void OnLaserHit(HurtboxComponent hurtbox)
	{
		QueueFree();
	}
}
