using Godot;
using System;

[GlobalClass]
public partial class MainMenu : Control
{
    [Export] Button startButton;
    [Export] Button settingsButton;
    [Export] Button quitButton;
    [Export] AnimationPlayer animationPlayer;
    private string _nextScenePath;

    public override void _Ready()
    {
        startButton.Pressed += OnStartPressed;
        settingsButton.Pressed += OnSettingsPressed;
        quitButton.Pressed += OnQuitPressed;
    }

    private void OnStartPressed()
    {

        _nextScenePath = "res://levels/level1/level_1.tscn";
        animationPlayer.Play("FadeOut");
        MenuBackground.Instance.BlockInput();
    }

    private void OnSettingsPressed()
    {
        MenuBackground.Instance.FadeInSmooth();
        MenuBackground.Instance.UnblockInput();

    }

    private void OnQuitPressed()
    {
        GD.Print("Quitting...");
        GetTree().Quit();
    }

    public void ChangeScene()
    {
        GetTree().ChangeSceneToFile(_nextScenePath);
    }
}
