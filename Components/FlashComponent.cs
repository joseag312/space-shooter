using Godot;

[GlobalClass]
public partial class FlashComponent : Node
{
    [Export] private Node2D _sprite;
    [Export] private float _flashDuration = 0.1f;

    public float FlashDuration
    {
        get => _flashDuration;
        set => _flashDuration = value;
    }

    public void Flash()
    {
        if (_sprite == null)
        {
            GD.PrintErr("ERROR: FlashComponent - Sprite not assigned to Flash Component");
            return;
        }

        Tween tween = GetTree().CreateTween();
        _sprite.Modulate = new Color(10, 10, 10, 1);
        tween.TweenProperty(_sprite, "modulate", new Color(1, 1, 1, 1), _flashDuration);
    }
}
