using Godot;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class MenuMain : Control
{
    [Export] public Button StartButton { get; set; }
    [Export] public Button SettingsButton { get; set; }
    [Export] public Button QuitButton { get; set; }
    [Export] public MenuFadeComponent MenuFadeComponent { get; set; }

    private string _nextScenePath;

    public override void _Ready()
    {
        StartButton.Pressed += OnStartPressed;
        SettingsButton.Pressed += OnSettingsPressed;
        QuitButton.Pressed += OnQuitPressed;
    }

    private async void OnStartPressed()
    {
        await MenuFadeComponent.FadeOutAsync();
        await G.GF.FadeToSceneWithLoading("res://levels/level1/level_1.tscn");
    }

    private async void OnSettingsPressed()
    {
        await MenuFadeComponent.FadeOutAsync();
        await G.GF.FadeToSceneKeepBG("res://levels/level1/level_1.tscn");
    }

    private async void OnQuitPressed()
    {
        await MenuFadeComponent.FadeOutAsync();
        await G.GF.FadeToSceneFadeBG("");
    }
}
