using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class HUDMain : CanvasLayer
{
    [Export] public HUDPowers Powers { get; set; }
    [Export] public HUDHealthBar HealthBar { get; set; }
    [Export] public HUDCurrency Currency { get; set; }
    [Export] public HUDDialogSystem Dialog { get; set; }

    public override void _Ready()
    {
        ResetHUDTransparency();
    }

    public async Task FirstMessage(string speakerKey, string mood, string message)
    {
        await FadeOutHUD();
        await Dialog.FirstMessage(speakerKey, mood, message);
    }

    public async Task Message(string speakerKey, string mood, string message)
    {
        await Dialog.Message(speakerKey, mood, message);
    }

    public async Task LastMessage(string speakerKey, string mood, string message)
    {
        await Dialog.LastMessage(speakerKey, mood, message);
        await FadeInHUD();
    }

    public async Task<string> MessageWithPrompt(
        string speakerKey,
        string mood = Mood.DEFAULT,
        string message = "",
        string optimistic = "",
        string pessimistic = "")
    {
        return await Dialog.MessageWithPrompt(speakerKey, mood, message, optimistic, pessimistic);
    }

    public async Task HandleChoice(
        string choice,
        string reactionSpeakerKey,
        string sureLine,
        string sureReaction,
        string optimistLine,
        string optimistReaction,
        string pessimistLine,
        string pessimistReaction)
    {
        await Dialog.HandleChoice(
            choice,
            reactionSpeakerKey,
            sureLine,
            sureReaction,
            optimistLine,
            optimistReaction,
            pessimistLine,
            pessimistReaction);
    }


    public async Task PopUpMessage(string speakerKey, string mood, string message, float duration = 1.5f)
    {
        await Dialog.PopUpMessage(speakerKey, mood, message, duration);
    }

    public async Task FadeOutHUD()
    {
        var tasks = new List<Task>();

        if (IsInstanceValid(HealthBar))
            tasks.Add(FadeNode(HealthBar, false));
        if (IsInstanceValid(Powers))
            tasks.Add(FadeNode(Powers, false));
        if (IsInstanceValid(Currency))
            tasks.Add(FadeNode(Currency, false));

        await Task.WhenAll(tasks);
    }

    public async Task FadeInHUD()
    {
        var tasks = new List<Task>();

        if (IsInstanceValid(HealthBar))
            tasks.Add(FadeNode(HealthBar, true));
        if (IsInstanceValid(Powers))
            tasks.Add(FadeNode(Powers, true));
        if (IsInstanceValid(Currency))
            tasks.Add(FadeNode(Currency, true));

        await Task.WhenAll(tasks);
    }

    private async Task FadeNode(CanvasItem node, bool fadeIn)
    {
        if (!IsInstanceValid(node)) return;

        var tween = GetTree().CreateTween();
        tween.TweenProperty(node, "modulate:a", fadeIn ? 1.0f : 0.0f, 0.8f)
             .SetTrans(Tween.TransitionType.Sine);

        await ToSignal(tween, "finished");
    }

    public void ResetHUDTransparency()
    {
        if (IsInstanceValid(HealthBar))
            HealthBar.Modulate = new Color(1, 1, 1, 0);
        if (IsInstanceValid(Powers))
            Powers.Modulate = new Color(1, 1, 1, 0);
        if (IsInstanceValid(Currency))
            Currency.Modulate = new Color(1, 1, 1, 0);
    }
}

