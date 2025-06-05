using Godot;
using System;

public partial class AutoBackground : ParallaxBackground
{
    public static AutoBackground Instance { get; private set; }

    [Export] private ParallaxLayer _background;
    [Export] private ParallaxLayer _slowLayer;
    [Export] private ParallaxLayer _fastLayer;

    [Export] private ColorRect _fadeBlack;
    [Export] private ColorRect _fadeWhite;
    [Export] private AnimatedSprite2D _loading;

    [Export] private Control _inputBlocker;

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

    public void ShowStars()
    {
        _background.Visible = true;
        _slowLayer.Visible = true;
        _fastLayer.Visible = true;

        _background.Modulate = new Color(1, 1, 1, 1);
        _slowLayer.Modulate = new Color(1, 1, 1, 1);
        _fastLayer.Modulate = new Color(1, 1, 1, 1);
    }

    public void HideStars()
    {
        _background.Visible = false;
        _slowLayer.Visible = false;
        _fastLayer.Visible = false;

        _background.Modulate = new Color(1, 1, 1, 0);
        _slowLayer.Modulate = new Color(1, 1, 1, 0);
        _fastLayer.Modulate = new Color(1, 1, 1, 0);
    }

    public async void FadeInStars()
    {
        _background.Visible = true;
        _slowLayer.Visible = true;
        _fastLayer.Visible = true;

        var tween = GetTree().CreateTween();
        tween.TweenProperty(_background, "modulate:a", 1f, 1.2f);
        tween.TweenProperty(_slowLayer, "modulate:a", 1f, 0.8f);
        tween.TweenProperty(_fastLayer, "modulate:a", 1f, 0.5f);
        await ToSignal(tween, "finished");
    }

    public async void FadeOutStars()
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(_fastLayer, "modulate:a", 0f, 0.3f);
        tween.TweenProperty(_slowLayer, "modulate:a", 0f, 0.8f);
        tween.TweenProperty(_background, "modulate:a", 0f, 1.2f);
        await ToSignal(tween, "finished");

        _background.Visible = false;
        _slowLayer.Visible = false;
        _fastLayer.Visible = false;
    }

    public async void FadeInBlack(float duration = 0.5f)
    {
        _fadeBlack.Visible = true;

        var tween = GetTree().CreateTween();
        tween.TweenProperty(_fadeBlack, "modulate:a", 1f, duration);
        await ToSignal(tween, "finished");
    }

    public async void FadeOutBlack(float duration = 0.5f)
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(_fadeBlack, "modulate:a", 0f, duration);
        await ToSignal(tween, "finished");

        _fadeBlack.Visible = false;
    }

    public async void FadeInWhite(float duration = 0.5f)
    {
        _fadeWhite.Visible = true;

        var tween = GetTree().CreateTween();
        tween.TweenProperty(_fadeWhite, "modulate:a", 1f, duration);
        await ToSignal(tween, "finished");
    }

    public async void FadeOutWhite(float duration = 0.5f)
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(_fadeWhite, "modulate:a", 0f, duration);
        await ToSignal(tween, "finished");

        _fadeWhite.Visible = false;
    }

    public async void FadeInLoading(float duration = 0.5f)
    {
        _loading.Visible = true;

        var tween = GetTree().CreateTween();
        tween.TweenProperty(_loading, "modulate:a", 1f, duration);
        await ToSignal(tween, "finished");
    }

    public async void FadeOutLoading(float duration = 0.5f)
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(_loading, "modulate:a", 0f, duration);
        await ToSignal(tween, "finished");

        _loading.Visible = false;
    }

    public void BlockInput()
    {
        _inputBlocker.Visible = true;
    }

    public void UnblockInput()
    {
        _inputBlocker.Visible = false;
    }
}
