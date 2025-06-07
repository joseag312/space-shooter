using Godot;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class MenuFadeComponent : Node
{
    [Export] public CenterContainer CanvasContainer;
    [Export] public float FadeDuration = 0.5f;

    public override void _Ready() { }

    public async Task FadeOutAsync()
    {
        if (CanvasContainer == null)
        {
            GD.PrintErr("ERROR: MenuFadeComponent - CanvasContainer is not set");
            return;
        }

        CanvasContainer.Visible = true;
        CanvasContainer.Modulate = new Color(1, 1, 1, 1);

        var tween = GetTree().CreateTween();
        tween.TweenProperty(CanvasContainer, "modulate:a", 0f, FadeDuration);

        await ToSignal(tween, "finished");

        AutoBackground.Instance.UnblockInput();
        AutoGameFlow.Instance.ResetTransition();
    }
}

