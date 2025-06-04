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
        BlockInput();
        startButton.Pressed += OnStartPressed;
        settingsButton.Pressed += OnSettingsPressed;
        quitButton.Pressed += OnQuitPressed;
        MainMenuLoad();
    }

    // Button functionality
    private void OnStartPressed()
    {
        _nextScenePath = "res://levels/level1/level_1.tscn";
        ButtonFadeOutAndSwitchScene();
    }

    private void OnSettingsPressed()
    {
        _nextScenePath = "res://levels/level1/level_1.tscn";
        ButtonFadeOutAndSwitchScene();
    }

    private void OnQuitPressed()
    {
        _nextScenePath = "";
        ButtonFadeOutAndSwitchScene();
    }

    // Animations
    public void MainMenuLoad()
    {
        animationPlayer.Play("MainMenuLoad");
    }

    public void ButtonFadeOutAndSwitchScene()
    {
        BlockInput();
        animationPlayer.Play("MenuFadeOut");
    }

    public void ChangeScene()
    {
        if ("".Equals(_nextScenePath))
        {
            GetTree().Quit();
        }
        GetTree().ChangeSceneToFile(_nextScenePath);
    }

    // Hooks for MenuBackground Singleton
    public void BlockInput()
    {
        MenuBackground.Instance.BlockInput();
    }

    public void UnblockInput()
    {
        MenuBackground.Instance.UnblockInput();
    }

    public void ShowStars()
    {
        MenuBackground.Instance.ShowStars();
    }

    public void HideStars()
    {
        MenuBackground.Instance.HideStars();
    }

    public void FadeInStars()
    {
        MenuBackground.Instance.FadeInStars();
    }

    public void FadeOutStars()
    {
        MenuBackground.Instance.FadeOutStars();
    }

    public void FadeInBlack(float duration = 1f)
    {
        MenuBackground.Instance.FadeInBlack(duration);
    }

    public void FadeOutBlack(float duration = 1f)
    {
        MenuBackground.Instance.FadeOutBlack(duration);
    }

    public void FadeInWhite(float duration = 1f)
    {
        MenuBackground.Instance.FadeInWhite(duration);
    }

    public void FadeOutWhite(float duration = 3f)
    {
        MenuBackground.Instance.FadeOutWhite(duration);
    }

    public void FadeInLoading(float duration = 1f)
    {
        MenuBackground.Instance.FadeInLoading(duration);
    }

    public void FadeOutLoading(float duration = 1f)
    {
        MenuBackground.Instance.FadeOutLoading(duration);
    }
}
