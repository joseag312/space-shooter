using Godot;
using System.Threading.Tasks;

[GlobalClass]
public partial class ShipDeathHandlerComponent : Node
{
    [Export] public Ship Ship { get; set; }
    [Export] public SpawnerRecurrentComponent SpawnerRecurrent { get; set; }
    [Export] public LevelCleanupComponent LevelCleanup { get; set; }
    [Export] public BackgroundFadeComponent BackgroundFade { get; set; }

    public void ConnectShipDeath()
    {
        Ship.StatsComponent.Connect("NoHealth", new Callable(this, nameof(OnShipDeath)));
    }

    private async void OnShipDeath()
    {
        SpawnerRecurrent.StopAllSpawning();

        var fadeBlackTask = G.BG.FadeInBlack(0.3f);
        var cleanupTask = LevelCleanup.FadeOutAll();
        var backgroundTask = BackgroundFade.FadeOutAll(0.3f);

        await Task.WhenAll(fadeBlackTask, cleanupTask, backgroundTask);
        await G.GF.FadeToSceneWithBGFast(G.GF.GameOverScene);
    }
}
