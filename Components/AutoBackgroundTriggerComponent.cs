using Godot;
using System;

[GlobalClass]
public partial class AutoBackgroundTriggerComponent : Node
{
    public void BlockInput()
    {
        AutoBackground.Instance.BlockInput();
    }

    public void UnblockInput()
    {
        AutoBackground.Instance.UnblockInput();
    }

    public void ShowStars()
    {
        AutoBackground.Instance.ShowStars();
    }

    public void HideStars()
    {
        AutoBackground.Instance.HideStars();
    }

    public void FadeInStars()
    {
        AutoBackground.Instance.FadeInStars();
    }

    public void FadeOutStars()
    {
        AutoBackground.Instance.FadeOutStars();
    }

    public void FadeInBlack(float duration = 0.5f)
    {
        AutoBackground.Instance.FadeInBlack(duration);
    }

    public void FadeOutBlack(float duration = 0.5f)
    {
        AutoBackground.Instance.FadeOutBlack(duration);
    }

    public void FadeInWhite(float duration = 0.5f)
    {
        AutoBackground.Instance.FadeInWhite(duration);
    }

    public void FadeOutWhite(float duration = 0.5f)
    {
        AutoBackground.Instance.FadeOutWhite(duration);
    }

    public void FadeInLoading(float duration = 0.5f)
    {
        AutoBackground.Instance.FadeInLoading(duration);
    }

    public void FadeOutLoading(float duration = 0.5f)
    {
        AutoBackground.Instance.FadeOutLoading(duration);
    }
}
