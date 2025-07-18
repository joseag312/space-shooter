using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial class MenuLevels : Control
{
    [Export] public MenuFadeComponent MenuFadeComponent;
    [Export] public HUDDialogSystem HUD;
    [Export] public LevelCard LevelCard;
    [Export] public Button ReturnButton;

    public override void _Ready()
    {
        LevelCatalog.LoadAll();
        foreach (var planet in GetTree().GetNodesInGroup("planet_panels"))
        {
            if (planet is PlanetPanel panel)
            {
                panel.PlanetClicked += OnPlanetClicked;
            }
        }
        ReturnButton.Pressed += OnReturnPressed;
    }

    private void OnPlanetClicked(string levelKey)
    {
        LevelCard.LoadLevel(levelKey);
    }

    private async Task testDialog(CancellationToken token)
    {
        await HUD.FirstMessage(Char.COMMANDER, Mood.COMMANDER.Annoyed, "Wussup");
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Annoyed, "Imma touch you");
        await HUD.LastMessage(Char.COMMANDER, Mood.COMMANDER.Annoyed, "That's all");
    }

    private async void OnReturnPressed()
    {
        await MenuFadeComponent.FadeOutAsync();
        await G.GF.FadeToSceneBasic(G.GF.MenuMainScene);
    }
}
