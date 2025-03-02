using Godot;

[GlobalClass]
public partial class MoveInputComponent : Node
{
    [Export] public MoveStats MoveStats { get; set; }
    [Export] public MoveComponent MoveComponent { get; set; }

    public override void _Input(InputEvent @event)
    {
        float inputAxis = Input.GetAxis("ui_left", "ui_right");
        MoveComponent.Velocity = new Vector2(inputAxis * MoveStats.Speed, 0);
    }
}
