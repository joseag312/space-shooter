using Godot;

public partial class Enemy : Node2D
{
	[Export] private StatsComponent statsComponent;
	[Export] private MoveComponent moveComponent;
	[Export] private ScaleComponent scaleComponent;
	[Export] private FlashComponent flashComponent;
	[Export] private ShakeComponent shakeComponent;
	[Export] private VisibleOnScreenNotifier2D visibleOnScreenNotifier2D;
	[Export] private HurtboxComponent hurtboxComponent;
	[Export] private HitboxComponent hitboxComponent;
	[Export] private TextureProgressBar healthBar;
	[Export] private AnimatedSprite2D animatedSprite;
	private float healthBarHeightOffset = 10f;

	public override void _Ready()
	{
		healthBar.MaxValue = statsComponent.Health;
		healthBar.Value = statsComponent.Health;
		hurtboxComponent.Hurt += OnHurt;
		statsComponent.NoHealth += IsDead;

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

		// Update health bar width
		healthBar.TextureUnder = gradientTextureUnder;
		healthBar.TextureProgress = gradientTextureProgress;

		// Reposition health bar above the sprite
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
		shakeComponent.TweenShake();
		UpdateHealthBar();
		SpawnDamageText((int)hitboxComponent.Damage);
	}

	private void SpawnDamageText(int damage)
	{
		PackedScene damageTextScene = (PackedScene)ResourceLoader.Load("res://enemies/damage_text.tscn");

		if (damageTextScene == null)
		{
			GD.PrintErr("ERROR: Enemy - DamageText.tscn could not be loaded!");
			return;
		}

		DamageText damageText = damageTextScene.Instantiate<DamageText>();

		if (damageText == null)
		{
			GD.PrintErr("ERROR: Enemy - Failed to instantiate DamageText!");
			return;
		}

		// Set position above the enemy in world space
		damageText.GlobalPosition = GlobalPosition + new Vector2(0, -10);

		// Initialize with damage value
		damageText.Initialize(damage);

		// Add to the scene
		GetParent().AddChild(damageText);
	}


	private void IsDead()
	{
		QueueFree();
	}

	public override void _ExitTree()
	{
		hurtboxComponent.Hurt -= OnHurt;
		statsComponent.NoHealth -= IsDead;
	}
}
