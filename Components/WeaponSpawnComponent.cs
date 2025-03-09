using Godot;

[GlobalClass]
public partial class WeaponSpawnComponent : Node
{
	[Export] private ScaleComponent scaleComponent;
	[Export] private FlashComponent flashComponent;
	[Export] private HitboxComponent hitboxComponent;

	public override void _Ready()
	{
		flashComponent.Flash();
		scaleComponent.TweenScale();
		hitboxComponent.HitHurtbox += OnWeaponHit;
	}

	private void OnWeaponHit(HurtboxComponent hurtbox)
	{
		GetParent().QueueFree();
	}
}

