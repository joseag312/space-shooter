using Godot;
using System.Threading.Tasks;

[GlobalClass]
public partial class ShipDeathHandlerComponent : Node
{
    [Export] public Ship Ship { get; set; }
    [Export] public SpawnerRecurrentComponent SpawnerRecurrent { get; set; }
    [Export] public LevelCleanupComponent LevelCleanup { get; set; }
    [Export] public ParallaxBackground ParallaxBackground { get; set; }
    [Export] public Node2D EffectContainer;

    private BackgroundFadeComponent _backgroundFade;

    public override void _Ready()
    {
        _backgroundFade = ParallaxBackground.GetNodeOrNull<BackgroundFadeComponent>("BackgroundFadeComponent");
        ConnectShipDeath();
    }

    public void ConnectShipDeath()
    {
        var callable = new Callable(this, nameof(OnShipDeath));
        if (!Ship.StatsComponent.IsConnected("NoHealth", callable))
            Ship.StatsComponent.Connect("NoHealth", callable);
        if (Ship.HasNode("DestroyedComponent"))
        {
            var destroy = Ship.GetNode<DestroyedComponent>("DestroyedComponent");
            destroy.EffectTarget = EffectContainer;
        }
    }

    private async void OnShipDeath()
    {
        SpawnerRecurrent.StopAllSpawning();

        var fadeBlackTask = G.BG.FadeInBlack(0.3f);
        var cleanupTask = LevelCleanup.FadeOutAll();
        var backgroundTask = _backgroundFade.FadeOutAll(0.3f);

        await Task.WhenAll(fadeBlackTask, cleanupTask, backgroundTask);
        await G.GF.FadeToSceneWithBGFast(G.GF.GameOverScene);
    }
}
