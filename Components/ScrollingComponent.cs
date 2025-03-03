using Godot;
using System;

public partial class ScrollingComponent : Node
{
	[Export] private ParallaxLayer space;
	[Export] private ParallaxLayer farStars;
	[Export] private ParallaxLayer closeStars;
	[Export] private int spaceSpeed = 1;
	[Export] private int farStarsSpeed = 5;
	[Export] private int closeStarsSpeed = 30;

	private Vector2 spaceOffset;
	private Vector2 farStarsOffset;
	private Vector2 closeStarsOffset;

	public override void _Ready()
	{
		spaceOffset = space.MotionOffset;
		farStarsOffset = farStars.MotionOffset;
		closeStarsOffset = closeStars.MotionOffset;
	}

	public override void _Process(double delta)
	{
		spaceOffset.Y += (float)delta * spaceSpeed;
		farStarsOffset.Y += (float)delta * farStarsSpeed;
		closeStarsOffset.Y += (float)delta * closeStarsSpeed;

		space.MotionOffset = spaceOffset;
		farStars.MotionOffset = farStarsOffset;
		closeStars.MotionOffset = closeStarsOffset;
	}
}
