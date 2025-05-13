using Godot;

public partial class ShipDamageText : Node2D
{
	[Export] private Label label;
	[Export] private float moveSpeed = 40f;
	[Export] private float fadeDuration = 0.3f;

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
		float randomXOffset = (float)GD.RandRange(-1d, 1d) * 10f;
		float randomYOffset = (float)GD.RandRange(0.5d, 1d) * 15f;

		int direction = GD.RandRange(0, 1) == 0 ? -1 : 1;
		float damageOffset = 24 * ((damageValue / 10) + 1) * direction;

		Position += new Vector2(damageOffset, 0);
		Vector2 targetPosition = Position + new Vector2(randomXOffset, -randomYOffset);
		tween.TweenProperty(this, "position", targetPosition, fadeDuration);
		tween.TweenProperty(this, "modulate:a", 0, fadeDuration).SetTrans(Tween.TransitionType.Linear);
		tween.TweenCallback(Callable.From(QueueFree));
	}
}
