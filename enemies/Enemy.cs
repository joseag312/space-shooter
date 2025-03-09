using Godot;

public partial class Enemy : Node2D
{
	[Export] private StatsComponent statsComponent;
	[Export] private ScaleComponent scaleComponent;
	[Export] private FlashComponent flashComponent;
	[Export] private HurtboxComponent hurtboxComponent;
	[Export] private TextureProgressBar healthBar;
	[Export] private AnimatedSprite2D animatedSprite;
	private float healthBarHeightOffset = 10f;

	public override void _Ready()
	{
		healthBar.MaxValue = statsComponent.Health;
		healthBar.Value = statsComponent.Health;
		hurtboxComponent.Hurt += OnHurt;

		AdjustHealthBar();
	}

	private void AdjustHealthBar()
	{
		if (animatedSprite == null || healthBar == null)
			return;

		SpriteFrames frames = animatedSprite.SpriteFrames;
		if (!frames.HasAnimation("move"))
			return;

		Texture2D frameTexture = animatedSprite.SpriteFrames.GetFrameTexture("move", 0);
		if (frameTexture == null)
			return;

		Vector2 spriteSize = frameTexture.GetSize() * animatedSprite.Scale;

		float minWidth = 50f;
		int newHealthBarWidth = (int)Mathf.Max(spriteSize.X, minWidth);

		Gradient gradientUnder = new Gradient();
		gradientUnder.SetColor(0, new Color(1, 0, 0, 1));
		gradientUnder.SetColor(1, new Color(1, 0, 0, 1));
		GradientTexture2D gradientTextureUnder = new GradientTexture2D
		{
			Gradient = gradientUnder,
			Width = newHealthBarWidth,
			Height = 5
		};

		Gradient gradientProgress = new Gradient();
		gradientProgress.SetColor(0, new Color(0, 1, 0, 1));
		gradientProgress.SetColor(1, new Color(0, 1, 0, 1));
		GradientTexture2D gradientTextureProgress = new GradientTexture2D
		{
			Gradient = gradientProgress,
			Width = newHealthBarWidth,
			Height = 5
		};

		healthBar.TextureUnder = gradientTextureUnder;
		healthBar.TextureProgress = gradientTextureProgress;
		healthBar.Position = new Vector2(-healthBar.TextureUnder.GetSize().X / 2, -spriteSize.Y / 2 - healthBarHeightOffset);
	}

	private void UpdateHealthBar()
	{
		healthBar.Value = statsComponent.Health;
	}

	private void OnHurt(HitboxComponent hitboxComponent)
	{
		scaleComponent.TweenScale();
		flashComponent.Flash();
		UpdateHealthBar();
	}

	public override void _ExitTree()
	{
		hurtboxComponent.Hurt -= OnHurt;
	}
}
