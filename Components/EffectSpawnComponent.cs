using Godot;

[GlobalClass]
public partial class EffectSpawnComponent : Node
{
    [Export] private AnimatedSprite2D _animatedSprite;
    [Export] private HitboxComponent _hitboxComponent;

    public override void _Ready()
    {
        _animatedSprite.Play();
        _animatedSprite.AnimationFinished += OnAnimationFinished;
    }

    private void OnAnimationFinished()
    {
        GetParent().QueueFree();
    }
}
