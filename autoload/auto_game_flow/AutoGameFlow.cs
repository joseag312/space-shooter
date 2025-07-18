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
    private bool _isInputBlocked = false;
    public bool IsInputBlocked => _isInputBlocked;

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

    public void BlockInput()
    {
        AutoBackground.Instance.BlockInput();
        _isInputBlocked = true;
    }

    public void UnblockInput()
    {
        AutoBackground.Instance.UnblockInput();
        _isInputBlocked = false;
    }

    public void StartDialog()
    {
        _isInputBlocked = true;
    }

    public void StopDialog()
    {
        _isInputBlocked = false;
    }

    public async Task FadeToSceneBasic(string path, float fadeDuration = 0.3f)
    {
        if (_isTransitioning) return;
        _isTransitioning = true;

        AutoBackground.Instance.BlockInput();
        await AutoBackground.Instance.FadeInBlack(fadeDuration);
        ChangeSceneSafely(path);
    }

    public async Task FadeToSceneKeepBG(string path, float fadeDuration = 0.3f)
    {
        if (_isTransitioning) return;
        _isTransitioning = true;

        AutoBackground.Instance.BlockInput();
        await AutoBackground.Instance.FadeInBlack(fadeDuration);
        ChangeSceneSafely(path);
    }

    public async Task FadeToSceneWithBG(string path, float fadeDuration = 0.3f)
    {
        if (_isTransitioning) return;
        _isTransitioning = true;

        AutoBackground.Instance.BlockInput();
        await AutoBackground.Instance.FadeInStars();
        await AutoBackground.Instance.FadeInBlack(fadeDuration);
        ChangeSceneSafely(path);
    }

    public async Task FadeToSceneWithBGFast(string path)
    {
        if (_isTransitioning) return;
        _isTransitioning = true;

        AutoBackground.Instance.BlockInput();
        await AutoBackground.Instance.FadeInStars(0.4f, 0.3f, 0.2f);
        await AutoBackground.Instance.FadeInBlack(0.2f);
        ChangeSceneSafely(path);
    }

    public async Task FadeToSceneFadeBG(string path, float fadeDuration = 0.3f)
    {
        if (_isTransitioning) return;
        _isTransitioning = true;

        AutoBackground.Instance.BlockInput();
        await AutoBackground.Instance.FadeOutStars();
        await AutoBackground.Instance.FadeInBlack(fadeDuration);
        ChangeSceneSafely(path);
    }

    public async Task FadeToSceneWithLoading(string path, float fadeDuration = 0.1f, float loadingFade = 0.3f, float holdDelay = 0.3f
    )
    {
        if (_isTransitioning) return;
        _isTransitioning = true;

        AutoBackground.Instance.BlockInput();
        await AutoBackground.Instance.FadeOutStars(0.4f, 0.3f, 0.2f);
        await AutoBackground.Instance.FadeInBlack(fadeDuration);
        G.SFX.Play(SFX.MEOW);
        await AutoBackground.Instance.FadeInLoading(loadingFade);
        await ToSignal(GetTree().CreateTimer(holdDelay), "timeout");
        await AutoBackground.Instance.FadeOutLoading(loadingFade);
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
