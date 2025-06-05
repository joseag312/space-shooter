using Godot;
using System;
using System.Threading.Tasks;

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
        MenuFadeIn();
    }

    // Button functionality
    private void OnStartPressed()
    {
        _nextScenePath = "res://levels/level1/level_1.tscn";
        MenuFadeOut();
    }

    private void OnSettingsPressed()
    {
        _nextScenePath = "res://levels/level1/level_1.tscn";
        MenuFadeOut();
    }

    private void OnQuitPressed()
    {
        _nextScenePath = "";
        MenuFadeOut();
    }

    public void MenuFadeIn()
    {
        animationPlayer.Play("MainMenu/FadeIn");
    }

    public void MenuFadeOut()
    {
        animationPlayer.Play("MainMenu/FadeOut");
    }

    public void ChangeScene()
    {
        if ("".Equals(_nextScenePath))
        {
            GetTree().Quit();
        }
        GetTree().ChangeSceneToFile(_nextScenePath);
    }
}
