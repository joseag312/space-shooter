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
    [Export] public Button StoreButton;

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
        StoreButton.Pressed += OnStorePressed;
    }

    private void OnPlanetClicked(string levelKey)
    {
        LevelCard.LoadLevel(levelKey);
    }

    private async Task TestDialog(CancellationToken token)
    {
        await HUD.FirstMessage(Char.COMMANDER, Mood.COMMANDER.Annoyed, "Wussup");
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Annoyed, "Testing...");
        await HUD.LastMessage(Char.COMMANDER, Mood.COMMANDER.Annoyed, "That's all");
    }

    private async void OnStorePressed()
    {
        await MenuFadeComponent.FadeOutAsync();
        await G.GF.FadeToSceneBasic(G.GF.MenuStoreScene);
    }

    private async void OnReturnPressed()
    {
        await MenuFadeComponent.FadeOutAsync();
        await G.GF.FadeToSceneBasic(G.GF.MenuMainScene);
    }
}
