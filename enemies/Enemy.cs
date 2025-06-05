using Godot;

public partial class Enemy : Node2D
{
	[Export] private StatsComponent _statsComponent;
	[Export] private ScaleComponent _scaleComponent;
	[Export] private FlashComponent _flashComponent;
	[Export] private HurtboxComponent _hurtboxComponent;
	[Export] private TextureProgressBar _healthBar;
	[Export] private AnimatedSprite2D _animatedSprite;

	private const float HealthBarHeightOffset = 10f;

	public override void _Ready()
	{
		_healthBar.MaxValue = _statsComponent.Health;
		_healthBar.Value = _statsComponent.Health;
		_hurtboxComponent.Hurt += OnHurt;
		AddToGroup("despawnable");

		AdjustHealthBar();
	}

	private void AdjustHealthBar()
	{
		SpriteFrames frames = _animatedSprite.SpriteFrames;
		Texture2D frameTexture = frames.GetFrameTexture("move", 0);
		Vector2 spriteSize = frameTexture.GetSize() * _animatedSprite.Scale;

		float minWidth = 50f;
		int newHealthBarWidth = (int)Mathf.Max(spriteSize.X, minWidth);

		var gradientUnder = new Gradient();
		gradientUnder.SetColor(0, new Color(1, 0, 0, 1));
		gradientUnder.SetColor(1, new Color(1, 0, 0, 1));

		var gradientProgress = new Gradient();
		gradientProgress.SetColor(0, new Color(0, 1, 0, 1));
		gradientProgress.SetColor(1, new Color(0, 1, 0, 1));

		var gradientTextureUnder = new GradientTexture2D
		{
			Gradient = gradientUnder,
			Width = newHealthBarWidth,
			Height = 5
		};

		var gradientTextureProgress = new GradientTexture2D
		{
			Gradient = gradientProgress,
			Width = newHealthBarWidth,
			Height = 5
		};

		_healthBar.TextureUnder = gradientTextureUnder;
		_healthBar.TextureProgress = gradientTextureProgress;
		_healthBar.Position = new Vector2(-gradientTextureUnder.GetSize().X / 2, -spriteSize.Y / 2 - HealthBarHeightOffset);
	}

	private void UpdateHealthBar()
	{
		_healthBar.Value = _statsComponent.Health;
	}

	private void OnHurt(HitboxComponent hitboxComponent)
	{
		_scaleComponent.TweenScale();
		_flashComponent.Flash();
		UpdateHealthBar();
	}

	public override void _ExitTree()
	{
		_hurtboxComponent.Hurt -= OnHurt;
	}
}
