using Godot;

public partial class ShipDamageText : Node2D
{
	[Export] private Label _label;
	[Export] private float _moveSpeed = 40f;
	[Export] private float _fadeDuration = 0.3f;

	private int _damageValue;
	private bool _initialized = false;

	public void Initialize(int damage)
	{
		_damageValue = damage;
		_initialized = true;
	}

	public override void _Ready()
	{
		if (!_initialized) return;

		if (_label == null)
		{
			GD.PrintErr("ERROR: ShipDamageText - Label is not assigned in DamageText!");
			return;
		}

		_label.Text = _damageValue.ToString();

		Tween tween = CreateTween();
		float randomXOffset = (float)GD.RandRange(-1d, 1d) * 10f;
		float randomYOffset = (float)GD.RandRange(0.5d, 1d) * 15f;

		int direction = GD.RandRange(0, 1) == 0 ? -1 : 1;
		float damageOffset = 24 * ((_damageValue / 10) + 1) * direction;

		Position += new Vector2(damageOffset, 0);
		Vector2 targetPosition = Position + new Vector2(randomXOffset, -randomYOffset);

		tween.TweenProperty(this, "position", targetPosition, _fadeDuration);
		tween.TweenProperty(this, "modulate:a", 0, _fadeDuration)
			.SetTrans(Tween.TransitionType.Linear);
		tween.TweenCallback(Callable.From(QueueFree));
	}
}
