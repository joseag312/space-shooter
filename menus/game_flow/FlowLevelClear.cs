using Godot;
using System;
using System.Threading.Tasks;

public partial class FlowLevelClear : CanvasLayer
{
    [Export] public Label MessageLabel;
    [Export] public MenuFadeComponent MenuFadeComponent { get; set; }

    private readonly string[] _sassyMessages =
    {
        "With my help, obviously. Sure..",
        "You survived?? Sure..",
        "I purred for luck.. wait no, food",
        "You held the controls, I held the glory. Sure..",
        "Not bad for a non-feline..",
        "Don’t get used to it."
    };

    public override void _Ready()
    {
        G.GS.Load();

        string randomMessage = _sassyMessages[GD.Randi() % _sassyMessages.Length];
        MessageLabel.Text = randomMessage;
        _ = RunFlowAsync();
    }

    private async Task RunFlowAsync()
    {
        await Task.Delay(2500);
        await MenuFadeComponent.FadeOutAsync();
        await G.GF.FadeToSceneBasic(G.GF.MenuMainScene, 0.1f);
    }
}