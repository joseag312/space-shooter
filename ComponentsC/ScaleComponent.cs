using Godot;
using System;

public partial class ScaleComponent : Node
{
	[Export] Node2D sprite;
	[Export] Vector2 scaleAmount = new Vector2(1.5f, 1.5f);
	[Export] float scaleDuration = 0.4f;

	public void TweenScale()
	{
		Tween tween = GetTree().CreateTween();
		tween.SetTrans(Tween.TransitionType.Expo);
		tween.SetEase(Tween.EaseType.Out);
		tween.TweenProperty(sprite, Node2D.PropertyName.Scaale.ToString(), scaleAmount, scaleDuration * 0.1f).FromCurrent();
		tween.TweenProperty(sprite, Node2D.PropertyName.Scale.ToString(), Vector2.One, scaleDuration * 0.9f).From(scaleAmount);
	}
}
