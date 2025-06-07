using Godot;
using System.Threading.Tasks;

public partial class AutoGameFlow : Node
{
    public static AutoGameFlow Instance { get; private set; }

    public string LastPlayedScene { get; set; } = "";
    public string MenuMainScene = "res://menus/menu_main/menu_main.tscn";
    public string GameOverScene = "res://menus/game_flow/flow_game_over.tscn";
    public string LevelClearScene = "res://menus/game_flow/flow_level_clear.tscn";

    private bool _isTransitioning = false;

    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;
            ProcessMode = ProcessModeEnum.Always;
        }
        else
        {
            QueueFree();
        }
    }

    public async Task FadeToSceneBasic(string path)
    {
        if (_isTransitioning) return;
        _isTransitioning = true;

        AutoBackground.Instance.BlockInput();
        await AutoBackground.Instance.FadeInBlack(0.5f);
        ChangeSceneSafely(path);
    }

    public async Task FadeToSceneKeepBG(string path)
    {
        if (_isTransitioning) return;
        _isTransitioning = true;

        AutoBackground.Instance.BlockInput();
        await AutoBackground.Instance.FadeInBlack(0.5f);
        ChangeSceneSafely(path);
    }

    public async Task FadeToSceneWithBG(string path)
    {
        if (_isTransitioning) return;
        _isTransitioning = true;

        AutoBackground.Instance.BlockInput();
        await AutoBackground.Instance.FadeInStars();
        await AutoBackground.Instance.FadeInBlack(0.5f);
        ChangeSceneSafely(path);
    }

    public async Task FadeToSceneWithBGFast(string path)
    {
        if (_isTransitioning) return;
        _isTransitioning = true;

        AutoBackground.Instance.BlockInput();
        await AutoBackground.Instance.FadeInStarsFast();
        await AutoBackground.Instance.FadeInBlack(0.2f);
        ChangeSceneSafely(path);
    }


    public async Task FadeToSceneFadeBG(string path)
    {
        if (_isTransitioning) return;
        _isTransitioning = true;

        AutoBackground.Instance.BlockInput();
        await AutoBackground.Instance.FadeOutStars();
        await AutoBackground.Instance.FadeInBlack(0.5f);
        ChangeSceneSafely(path);
    }

    public async Task FadeToSceneWithLoading(string path)
    {
        if (_isTransitioning) return;
        _isTransitioning = true;

        AutoBackground.Instance.BlockInput();
        await AutoBackground.Instance.FadeOutStars();
        await AutoBackground.Instance.FadeInBlack(0.5f);
        await AutoBackground.Instance.FadeInLoading(0.3f);
        await ToSignal(GetTree().CreateTimer(0.6f), "timeout");
        await AutoBackground.Instance.FadeOutLoading(0.3f);
        ChangeSceneSafely(path);
    }

    public void ResetTransition()
    {
        _isTransitioning = false;
    }

    private void ChangeSceneSafely(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            GetTree().Quit();
        }
        else
        {
            GetTree().ChangeSceneToFile(path);
        }
    }
}