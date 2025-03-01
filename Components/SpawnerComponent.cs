using Godot;

[GlobalClass]
public partial class SpawnerComponent : Node2D
{
    [Export] public PackedScene Scene { get; set; }

    public Node Spawn(Vector2 globalSpawnPosition, Node parent = null)
    {
        GD.Assert(Scene != null, "Error: The scene export was never set on this spawner component.");

        Node instance = Scene.Instantiate();
        parent ??= GetTree().CurrentScene;

        parent.AddChild(instance);
        if (instance is Node2D node2D)
        {
            node2D.GlobalPosition = globalSpawnPosition;
        }

        return instance;
    }
}
