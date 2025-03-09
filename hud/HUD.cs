using System;
using Godot;

public partial class HUD : Control
{
	[Export] public TextureProgressBar healthBar;
	[Export] public TextureProgressBar backgroundBar;
	[Export] private float lostHealthDuration = 0.5f;
	[Export] public Label healthValue;
	private StatsComponent shipStats;
	private Tween tween;

	public override void _Ready()
	{
		Node2D ship = GetTree().CurrentScene.GetNodeOrNull<Node2D>("Ship");
		if (ship != null)
		{
			shipStats = ship.GetNodeOrNull<StatsComponent>("StatsComponent");
			ship.Connect(nameof(Ship.HealthChanged), new Callable(this, nameof(OnHealthChanged)));
		}

		if (shipStats != null)
		{
			healthBar.MaxValue = shipStats.MaxHealth;
			healthBar.Value = shipStats.Health;
			healthValue.Text = shipStats.Health.ToString() + "/" + shipStats.MaxHealth.ToString();
		}

		healthBar.Size = new Vector2(200, 200);
		Position = new Vector2(20, 40);
	}

	private void OnHealthChanged(int damage)
	{
		if (shipStats == null) return;

		healthBar.Value = Mathf.Max(shipStats.Health, 0);
		healthValue.Text = healthBar.Value.ToString() + "/" + healthBar.MaxValue.ToString();

		Gradient gradientUnder = new Gradient();
		Color hurtColor = new Color(145f / 200, 125f / 255f, 230f / 255f, 1f);
		gradientUnder.SetColor(0, hurtColor);
		gradientUnder.SetColor(1, hurtColor);
		GradientTexture2D gradientTextureUnder = new GradientTexture2D
		{
			Gradient = gradientUnder,
			Width = (int)(backgroundBar.TextureUnder.GetWidth() * ((float)damage / (float)healthBar.MaxValue)),
			Height = backgroundBar.TextureUnder.GetHeight()
		};
		TextureProgressBar lostHealthBar = new TextureProgressBar
		{
			TextureProgress = gradientTextureUnder,
			MinValue = healthBar.MinValue,
			MaxValue = healthBar.MaxValue,
			Value = healthBar.MaxValue,
			TextureProgressOffset = new Vector2(backgroundBar.TextureUnder.GetWidth() * (shipStats.Health / (float)healthBar.MaxValue), 0f),
			ZIndex = 5
		};
		healthBar.GetParent().AddChild(lostHealthBar);
		lostHealthBar.GlobalPosition = healthBar.GlobalPosition;

		Tween tween = GetTree().CreateTween();
		if (shipStats.Health > 0)
		{
			float startOffsetX = lostHealthBar.TextureProgressOffset.X;
			float endOffsetX = startOffsetX - lostHealthBar.TextureProgress.GetWidth();
			tween.TweenProperty(lostHealthBar, "texture_progress_offset:x", endOffsetX, 0.4f)
						 .SetTrans(Tween.TransitionType.Linear)
						 .SetEase(Tween.EaseType.Out);
		}
		else
		{
			tween.TweenProperty(lostHealthBar, "modulate", new Color(1f, 1f, 2f, 1f), 0.1f)
				 .SetTrans(Tween.TransitionType.Cubic)
				 .SetEase(Tween.EaseType.Out);
			tween.TweenProperty(lostHealthBar, "modulate", new Color(2f, 2f, 2f, 1f), 0.1f)
				 .SetDelay(0.1f)
				 .SetTrans(Tween.TransitionType.Bounce)
				 .SetEase(Tween.EaseType.Out);
			tween.TweenProperty(lostHealthBar, "modulate", new Color(0.5f, 0.5f, 0.5f, 1f), 0.1f)
				 .SetDelay(0.2f)
				 .SetTrans(Tween.TransitionType.Cubic)
				 .SetEase(Tween.EaseType.Out);
			tween.TweenProperty(lostHealthBar, "modulate:a", 0f, 0.4f)
				 .SetDelay(0.3f)
				 .SetTrans(Tween.TransitionType.Linear);
		}
		tween.TweenCallback(Callable.From(() =>
		{
			if (IsInstanceValid(lostHealthBar)) lostHealthBar.QueueFree();
		})).SetDelay(0.6f);
	}
}
