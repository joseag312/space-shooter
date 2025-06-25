using Godot;

public partial class ShipHealText : Node2D
{
    [Export] private Label _label;
    [Export] private float _moveSpeed = 40f;
    [Export] private float _fadeDuration = 0.3f;

    private int _healValue;
    private bool _initialized = false;

    public void Initialize(int heal)
    {
        _healValue = heal;
        _initialized = true;
    }

    public override void _Ready()
    {
        if (!_initialized) return;

        if (_label == null)
        {
            GD.PrintErr("ERROR: ShipHealText - Label is not assigned in healText!");
            return;
        }

        _label.Text = _healValue.ToString();

        Tween tween = CreateTween();
        float randomXOffset = (float)GD.RandRange(-1d, 1d) * 50f;
        float randomYOffset = (float)GD.RandRange(0.5d, 1d) * 35f;

        int direction = GD.RandRange(0, 1) == 0 ? -1 : 1;
        float healOffset = 24 * ((_healValue / 10) + 1) * direction;

        Position += new Vector2(healOffset, 0);
        Vector2 targetPosition = Position + new Vector2(randomXOffset, -randomYOffset);

        tween.TweenProperty(this, "position", targetPosition, _fadeDuration);
        tween.TweenProperty(this, "modulate:a", 0, _fadeDuration)
            .SetTrans(Tween.TransitionType.Linear);
        tween.TweenCallback(Callable.From(QueueFree));
    }
}
