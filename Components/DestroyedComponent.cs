using Godot;

[GlobalClass]
public partial class DestroyedComponent : Node
{
    [Export] public Node2D Actor { get; set; }
    [Export] public StatsComponent StatsComponent { get; set; }
    [Export] public SpawnerComponent DestroyEffectSpawnerComponent { get; set; }

    public override void _Ready()
    {
        StatsComponent.NoHealth += Destroy;
    }

    private void Destroy()
    {
        DestroyEffectSpawnerComponent.Spawn(Actor.GlobalPosition);
        Actor.QueueFree();
    }
}
