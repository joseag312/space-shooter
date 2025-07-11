using Godot;

[GlobalClass]
public partial class TeleportWeaponComponent : Node2D, IEffectSpawningWeapon
{
    public Node2D EffectContainer { get; set; }

    private readonly PackedScene TeleportEffectScene = GD.Load<PackedScene>("res://player_projectiles/pp_teleport/pp_teleport_effect.tscn");
    private Node2D _ship;
    private MoveComponent _moveComponent;
    private FlashComponent _flashComponent;
    private InvincibilityComponent _invincibilityComponent;

    [Signal]
    public delegate void PPTeleportSuccessEventHandler();

    public const string EmitSignalName = "PPTeleportSuccess";

    [Export] public float TeleportDistance = 70f;

    public override void _Ready()
    {
        _ship = GetParent<Node2D>();
        _moveComponent = _ship.GetNode<MoveComponent>("MoveComponent");
        _flashComponent = _ship.GetNode<FlashComponent>("FlashComponent");
        _invincibilityComponent = _ship.GetNode<InvincibilityComponent>("InvincibilityComponent");

        CallDeferred(nameof(Teleport));
    }

    private void Teleport()
    {
        Vector2 velocity = _moveComponent.Velocity;

        if (velocity == Vector2.Zero)
        {
            return;
        }

        float margin = 48f;
        float left = 0 + margin;
        float right = (float)ProjectSettings.GetSetting("display/window/size/viewport_width") - margin;
        float top = 0 + margin;
        float bottom = (float)ProjectSettings.GetSetting("display/window/size/viewport_height") - margin;

        Vector2 direction = velocity.Normalized();
        Vector2 target = _ship.GlobalPosition + direction * TeleportDistance;

        if (target.X < left || target.X > right || target.Y < top || target.Y > bottom)
        {
            return;
        }

        if (TeleportEffectScene != null && EffectContainer != null)
        {
            Node2D effectInstance = (Node2D)TeleportEffectScene.Instantiate();
            effectInstance.GlobalPosition = _ship.GlobalPosition;
            EffectContainer.AddChild(effectInstance);
        }

        _invincibilityComponent.StartInvincibility();
        _flashComponent.Flash();
        _ship.GlobalPosition = target;

        EmitSignal(EmitSignalName);
    }
}
