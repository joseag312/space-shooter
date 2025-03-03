using Godot;
using System.Diagnostics;

[GlobalClass]
public partial class FlashComponent : Node
{
    [Export] public Node2D sprite;
    [Export] private float flashDuration = 0.1f; // Duration of the flash

    public void Flash()
    {
        if (sprite == null)
        {
            GD.PrintErr("ERROR: Sprite not assigned to Flash Component.");
            return;
        }

        Tween tween = GetTree().CreateTween();
        sprite.Modulate = new Color(10, 10, 10, 1);
        tween.TweenProperty(sprite, "modulate", new Color(1, 1, 1, 1), flashDuration);
    }
}
