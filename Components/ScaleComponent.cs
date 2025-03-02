using Godot;
using System;

[GlobalClass]
public partial class ScaleComponent : Node
{
	[Export] public Node2D sprite;
	[Export] public Vector2 scaleAmount = new Vector2(1.5f, 1.5f);
	[Export] public float scaleDuration = 0.5f;

	public void TweenScale()
	{
		Tween tween = GetTree().CreateTween();
		tween.SetTrans(Tween.TransitionType.Expo);
		tween.SetEase(Tween.EaseType.Out);
		tween.TweenProperty(sprite, Node2D.PropertyName.Scale.ToString(), scaleAmount, scaleDuration * 0.1f).FromCurrent();
		tween.TweenProperty(sprite, Node2D.PropertyName.Scale.ToString(), Vector2.One, scaleDuration * 0.9f).From(scaleAmount);
	}
}
