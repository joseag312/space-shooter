using Godot;
using System;

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
	}

	private void OnLaserHit(HurtboxComponent hurtbox)
	{
		GD.Print("Laser hit something! Despawning...");
		QueueFree();
	}



}
