using Godot;
using System;

public partial class BasicBlueLaser : Node2D
{
	[Export] private ScaleComponent scaleComponent;
	[Export] private FlashComponent flashComponent;
	public override void _Ready()
	{
		flashComponent.Flash();
		scaleComponent.TweenScale();
	}
}
