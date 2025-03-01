using Godot;

[GlobalClass]
public partial class OnetimeAnimatedEffect : AnimatedSprite2D
{
    public override void _Ready()
    {
        AnimationFinished += QueueFree;
    }
}
