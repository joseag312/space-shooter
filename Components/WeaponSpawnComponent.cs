using Godot;

[GlobalClass]
public partial class WeaponSpawnComponent : Node
{
	[Export] private ScaleComponent _scaleComponent;
	[Export] private FlashComponent _flashComponent;
	[Export] private HitboxComponent _hitboxComponent;

	public override void _Ready()
	{
		_flashComponent.Flash();
		_scaleComponent.TweenScale();
		_hitboxComponent.HitHurtbox += OnWeaponHit;
	}

	private void OnWeaponHit(HurtboxComponent hurtbox)
	{
		GetParent().QueueFree();
	}
}
