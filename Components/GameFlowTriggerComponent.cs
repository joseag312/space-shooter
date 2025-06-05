using Godot;
using System;

[GlobalClass]
public partial class GameFlowTriggerComponent : Node
{
    public void BlockInput()
    {
        GameFlow.Instance.BlockInput();
    }

    public void UnblockInput()
    {
        GameFlow.Instance.UnblockInput();
    }

    public void ShowStars()
    {
        GameFlow.Instance.ShowStars();
    }

    public void HideStars()
    {
        GameFlow.Instance.HideStars();
    }

    public void FadeInStars()
    {
        GameFlow.Instance.FadeInStars();
    }

    public void FadeOutStars()
    {
        GameFlow.Instance.FadeOutStars();
    }

    public void FadeInBlack(float duration = 0.5f)
    {
        GameFlow.Instance.FadeInBlack(duration);
    }

    public void FadeOutBlack(float duration = 0.5f)
    {
        GameFlow.Instance.FadeOutBlack(duration);
    }

    public void FadeInWhite(float duration = 0.5f)
    {
        GameFlow.Instance.FadeInWhite(duration);
    }

    public void FadeOutWhite(float duration = 0.5f)
    {
        GameFlow.Instance.FadeOutWhite(duration);
    }

    public void FadeInLoading(float duration = 0.5f)
    {
        GameFlow.Instance.FadeInLoading(duration);
    }

    public void FadeOutLoading(float duration = 0.5f)
    {
        GameFlow.Instance.FadeOutLoading(duration);
    }
}
