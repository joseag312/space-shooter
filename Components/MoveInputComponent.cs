using Godot;

[GlobalClass]
public partial class MoveInputComponent : Node
{
    [Export] public MoveComponent MoveComponent { get; set; }

    public override void _Input(InputEvent @event)
    {
        float inputAxisX = Input.GetAxis("ui_left", "ui_right");
        float inputAxisY = Input.GetAxis("ui_up", "ui_down");
        MoveComponent.Velocity = new Vector2(inputAxisX * ShipStats.Instance.Speed, inputAxisY * ShipStats.Instance.Speed);
    }
}
