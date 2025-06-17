using Godot;

[GlobalClass]
public partial class LevelFlowComponent : Node
{
    [Export] public ShipDeathHandlerComponent ShipDeathHandler { get; set; }
    [Export] public SpawnerRecurrentComponent SpawnerRecurrent { get; set; }
    [Export] public SpawnerWaveComponent SpawnerWave { get; set; }
    [Export] public LevelCleanupComponent LevelCleanup { get; set; }

    public override void _Ready()
    {
        G.BG.UnblockInput();
        G.GF.ResetTransition();
        G.GF.LastPlayedScene = GetTree().CurrentScene.SceneFilePath;
    }
}
