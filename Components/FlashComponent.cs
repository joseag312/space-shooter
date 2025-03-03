using Godot;
using System.Diagnostics;
using System.Threading.Tasks;

[GlobalClass]
public partial class FlashComponent : Node
{
    [Export] private NodePath spritePath;
    [Export] private float flashDuration = 0.1f; // Duration of the flash

    private Sprite2D sprite;

    public override void _Ready()
    {
        sprite = GetNodeOrNull<Sprite2D>(spritePath);
        if (sprite == null)
        {
            Debug.Print("FlashEffect: No Sprite2D found at " + spritePath);
            return;
        }
        Flash();
    }

    private async void Flash()
    {
        if (sprite == null) return;

        sprite.Modulate = new Color(2, 2, 2, 1);
        await Task.Delay((int)(flashDuration * 1000));
        sprite.Modulate = new Color(1, 1, 1, 1);
    }
}
