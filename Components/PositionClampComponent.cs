using Godot;

[GlobalClass]
public partial class PositionClampComponent : Node2D
{
    [Export] public Node2D Actor { get; set; }
    [Export] public float Margin { get; set; } = 8f;

    private readonly float _leftBorder = 0;
    private readonly float _rightBorder = (float)ProjectSettings.GetSetting("display/window/size/viewport_width");

    public override void _Process(double delta)
    {
        Actor.GlobalPosition = new Vector2(
            Mathf.Clamp(Actor.GlobalPosition.X, _leftBorder + Margin, _rightBorder - Margin),
            Actor.GlobalPosition.Y
        );
    }
}
