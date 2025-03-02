using Godot;

[GlobalClass]
public partial class MoveStats : Resource
{
    [Export] public int Speed { get; set; } = 150;
}
