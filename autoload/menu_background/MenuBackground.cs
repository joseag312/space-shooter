using Godot;
using System;

public partial class MenuBackground : ParallaxBackground
{
    public static MenuBackground Instance { get; private set; }

    [Export] private ParallaxLayer starLayer;
    [Export] private ParallaxLayer slowLayer;
    [Export] private ParallaxLayer fastLayer;
    [Export] private ColorRect fadeBlack;
    [Export] private ColorRect fadeWhite;
    [Export] private AnimatedSprite2D loading;
    [Export] private Control inputBlocker;

    public override void _Ready()
    {
        starLayer.Visible = false;
        slowLayer.Visible = false;
        fastLayer.Visible = false;

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

    public void ShowStars()
    {
        starLayer.Visible = true;
        slowLayer.Visible = true;
        fastLayer.Visible = true;

        starLayer.Modulate = new Color(1, 1, 1, 1);
        slowLayer.Modulate = new Color(1, 1, 1, 1);
        fastLayer.Modulate = new Color(1, 1, 1, 1);
    }

    public void HideStars()
    {
        starLayer.Visible = false;
        slowLayer.Visible = false;
        fastLayer.Visible = false;

        starLayer.Modulate = new Color(1, 1, 1, 0);
        slowLayer.Modulate = new Color(1, 1, 1, 0);
        fastLayer.Modulate = new Color(1, 1, 1, 0);
    }

    public async void FadeInStars()
    {
        starLayer.Visible = true;
        slowLayer.Visible = true;
        fastLayer.Visible = true;

        starLayer.Modulate = new Color(1, 1, 1, 0);
        slowLayer.Modulate = new Color(1, 1, 1, 0);
        fastLayer.Modulate = new Color(1, 1, 1, 0);

        var tween = GetTree().CreateTween();
        tween.TweenProperty(starLayer, "modulate:a", 1f, 1.2f);
        tween.TweenProperty(slowLayer, "modulate:a", 1f, 0.8f);
        tween.TweenProperty(fastLayer, "modulate:a", 1f, 0.3f);
        await ToSignal(tween, "finished");
    }

    public async void FadeOutStars()
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(fastLayer, "modulate:a", 0f, 0.3f);
        tween.TweenProperty(slowLayer, "modulate:a", 0f, 0.8f);
        tween.TweenProperty(starLayer, "modulate:a", 0f, 1.2f);
        await ToSignal(tween, "finished");

        starLayer.Visible = false;
        slowLayer.Visible = false;
        fastLayer.Visible = false;
    }

    public async void FadeInBlack(float duration = 0.5f)
    {
        fadeBlack.Visible = true;
        fadeBlack.Modulate = new Color(0, 0, 0, 0);

        var tween = GetTree().CreateTween();
        tween.TweenProperty(fadeBlack, "modulate:a", 1f, duration);
        await ToSignal(tween, "finished");
    }

    public async void FadeOutBlack(float duration = 0.5f)
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(fadeBlack, "modulate:a", 0f, duration);
        await ToSignal(tween, "finished");

        fadeBlack.Visible = false;
    }

    public async void FadeInWhite(float duration = 0.5f)
    {
        fadeWhite.Visible = true;
        fadeWhite.Modulate = new Color(1, 1, 1, 0);

        var tween = GetTree().CreateTween();
        tween.TweenProperty(fadeWhite, "modulate:a", 1f, duration);
        await ToSignal(tween, "finished");
    }

    public async void FadeOutWhite(float duration = 0.5f)
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(fadeWhite, "modulate:a", 0f, duration);
        await ToSignal(tween, "finished");

        fadeWhite.Visible = false;
    }

    public async void FadeInLoading(float duration = 0.5f)
    {
        if (loading == null) return;

        loading.Modulate = new Color(1, 1, 1, 0);
        loading.Visible = true;

        var tween = GetTree().CreateTween();
        tween.TweenProperty(loading, "modulate:a", 1f, duration);
        await ToSignal(tween, "finished");
    }

    public async void FadeOutLoading(float duration = 0.5f)
    {
        if (loading == null) return;

        var tween = GetTree().CreateTween();
        tween.TweenProperty(loading, "modulate:a", 0f, duration);
        await ToSignal(tween, "finished");

        loading.Visible = false;
    }

    public void BlockInput()
    {
        inputBlocker.Visible = true;
    }

    public void UnblockInput()
    {
        inputBlocker.Visible = false;
    }
}
