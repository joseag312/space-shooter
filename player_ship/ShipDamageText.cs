using Godot;

public partial class ShipDamageText : Node2D
{
	[Export] private Label label;
	[Export] private float moveSpeed = 80f;
	[Export] private float fadeDuration = 0.5f;

	private int damageValue;
	private bool initialized = false;

	public void Initialize(int damage)
	{
		damageValue = damage;
		initialized = true;
	}

	public override void _Ready()
	{
		if (!initialized) return;

		if (label == null)
		{
			GD.PrintErr("ERROR: ShipDamageText - Label is not assigned in DamageText!");
			return;
		}

		label.Text = damageValue.ToString();

		Tween tween = CreateTween();
		float randomXOffset = (float)GD.RandRange(-1, 1) * 80f;
		float randomYOffset = (float)GD.RandRange(-1, 1) * 80f;
		Vector2 startPosition = Position;
		if (damageValue < 10)
		{
			startPosition.X -= 12;
		}
		else
		{
			startPosition.X -= 24;
		}

		Vector2 targetPosition = startPosition + new Vector2(randomXOffset, -randomYOffset);
		tween.TweenProperty(this, "position", targetPosition, fadeDuration);
		tween.TweenProperty(this, "modulate:a", 0, fadeDuration).SetTrans(Tween.TransitionType.Linear);
		tween.TweenCallback(Callable.From(QueueFree));
	}
}
