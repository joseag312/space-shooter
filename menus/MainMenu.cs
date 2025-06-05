using Godot;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class MainMenu : Control
{
    [Export] public Button StartButton { get; set; }
    [Export] public Button SettingsButton { get; set; }
    [Export] public Button QuitButton { get; set; }
    [Export] public AnimationPlayer AnimationPlayer { get; set; }

    private string _nextScenePath;

    public override void _Ready()
    {
        StartButton.Pressed += OnStartPressed;
        SettingsButton.Pressed += OnSettingsPressed;
        QuitButton.Pressed += OnQuitPressed;

        MenuFadeIn();
    }

    private void OnStartPressed()
    {
        _nextScenePath = "res://levels/level1/level_1.tscn";
        MenuFadeOut();
    }

    private void OnSettingsPressed()
    {
        _nextScenePath = "res://levels/level1/level_1.tscn"; // Consider updating this path later
        MenuFadeOut();
    }

    private void OnQuitPressed()
    {
        _nextScenePath = "";
        MenuFadeOut();
    }

    public void MenuFadeIn()
    {
        AnimationPlayer.Play("MainMenu/FadeIn");
    }

    public void MenuFadeOut()
    {
        AnimationPlayer.Play("MainMenu/FadeOut");
    }

    public void ChangeScene()
    {
        if (_nextScenePath == "")
        {
            GetTree().Quit();
            return;
        }

        GetTree().ChangeSceneToFile(_nextScenePath);
    }
}
