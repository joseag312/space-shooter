using Godot;
using System;

public partial class MenuBackground : ParallaxBackground
{
    public static MenuBackground Instance { get; private set; }

    [Export] ParallaxLayer starLayer;
    [Export] ParallaxLayer slowLayer;
    [Export] ParallaxLayer fastLayer;
    [Export] ColorRect fadeBlack;
    [Export] Control inputBlocker;

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

    public void FadeOut()
    {
        starLayer.Visible = false;
        slowLayer.Visible = false;
        fastLayer.Visible = false;

        starLayer.Modulate = new Color(1, 1, 1, 0);
        slowLayer.Modulate = new Color(1, 1, 1, 0);
        fastLayer.Modulate = new Color(1, 1, 1, 0);
    }

    public async void FadeOutSmooth()
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

    public void FadeIn()
    {
        starLayer.Visible = true;
        slowLayer.Visible = true;
        fastLayer.Visible = true;

        starLayer.Modulate = new Color(1, 1, 1, 1);
        slowLayer.Modulate = new Color(1, 1, 1, 1);
        fastLayer.Modulate = new Color(1, 1, 1, 1);
    }

    public async void FadeInSmooth()
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

    public void BlockInput()
    {
        inputBlocker.Visible = true;
    }

    public void UnblockInput()
    {
        inputBlocker.Visible = false;
    }
}
