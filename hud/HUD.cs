using Godot;

public partial class HUD : Control
{
	[Export] public TextureProgressBar healthBar;
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
			healthBar.MaxValue = shipStats.Health;
			healthBar.Value = shipStats.Health;
			healthValue.Text = shipStats.Health.ToString();
		}

		healthBar.Size = new Vector2(200, 200);
		Position = new Vector2(20, 10);
	}

	private void OnHealthChanged(float damage)
	{
		if (shipStats == null) return;
		healthBar.Value = shipStats.Health;
		healthValue.Text = shipStats.Health.ToString();

		Gradient gradientUnder = new Gradient();
		gradientUnder.SetColor(0, new Color(50f / 255f, 160f / 255f, 220f / 255f, 1f));
		gradientUnder.SetColor(1, new Color(50f / 255f, 160f / 255f, 220f / 255f, 1f));
		GradientTexture2D gradientTextureUnder = new GradientTexture2D
		{
			Gradient = gradientUnder,
			Width = (int)(healthBar.TextureUnder.GetWidth() * (damage / (float)healthBar.MaxValue)),
			Height = healthBar.TextureUnder.GetHeight()
		};
		TextureProgressBar lostHealthBar = new TextureProgressBar
		{
			TextureProgress = gradientTextureUnder,
			MinValue = healthBar.MinValue,
			MaxValue = healthBar.MaxValue,
			Value = healthBar.MaxValue,
			TextureProgressOffset = new Vector2(healthBar.TextureUnder.GetWidth() * (shipStats.Health / (float)healthBar.MaxValue), 0f)
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

			// 1️⃣ Change color to yellow immediately
			tween.TweenProperty(lostHealthBar, "modulate", new Color(4f, 0.5f, 0.5f, 1f), 0.1f)
				 .SetTrans(Tween.TransitionType.Cubic)
				 .SetEase(Tween.EaseType.Out);

			// 2️⃣ Flash Effect (Brighten to White)
			tween.TweenProperty(lostHealthBar, "modulate", new Color(2f, 2f, 2f, 1f), 0.1f)
				 .SetDelay(0.1f)
				 .SetTrans(Tween.TransitionType.Bounce)
				 .SetEase(Tween.EaseType.Out);

			// 3️⃣ Return to Normal Yellow Before Fading
			tween.TweenProperty(lostHealthBar, "modulate", new Color(0.5f, 0.5f, 0.5f, 1f), 0.1f)
				 .SetDelay(0.2f)
				 .SetTrans(Tween.TransitionType.Cubic)
				 .SetEase(Tween.EaseType.Out);

			// 4️⃣ Fade Out (Alpha to 0)
			tween.TweenProperty(lostHealthBar, "modulate:a", 0f, 0.4f)
				 .SetDelay(0.3f)
				 .SetTrans(Tween.TransitionType.Linear);

			// 5️⃣ Queue Free After Fade
			tween.TweenCallback(Callable.From(() => lostHealthBar.QueueFree()))
				 .SetDelay(0.7f);

		}
		tween.TweenCallback(Callable.From(() => lostHealthBar.QueueFree()))
			 .SetDelay(0.6f);

	}
}
