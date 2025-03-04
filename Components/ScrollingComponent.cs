using Godot;
using System;

public partial class ScrollingComponent : Node
{
	[Export] private ParallaxLayer background;
	[Export] private ParallaxLayer slowLayer;
	[Export] private ParallaxLayer fastLayer;
	[Export] private int spaceSpeed = 1;
	[Export] private int farStarsSpeed = 5;
	[Export] private int closeStarsSpeed = 30;

	private Vector2 spaceOffset;
	private Vector2 farStarsOffset;
	private Vector2 closeStarsOffset;

	public override void _Ready()
	{
		spaceOffset = background.MotionOffset;
		farStarsOffset = slowLayer.MotionOffset;
		closeStarsOffset = fastLayer.MotionOffset;
	}

	public override void _Process(double delta)
	{
		spaceOffset.Y += (float)delta * spaceSpeed;
		farStarsOffset.Y += (float)delta * farStarsSpeed;
		closeStarsOffset.Y += (float)delta * closeStarsSpeed;

		background.MotionOffset = spaceOffset;
		slowLayer.MotionOffset = farStarsOffset;
		fastLayer.MotionOffset = closeStarsOffset;
	}
}
