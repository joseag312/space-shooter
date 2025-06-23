using Godot;
using System;

[GlobalClass]
public abstract partial class DropBase : Area2D
{
    [Export] public PackedScene FloatingTextScene;
    [Export] public PulseComponent PulseComponent;
    [Export] public HurtboxComponent HurtboxComponent;

    public override void _Ready()
    {
        AddToGroup("despawnable");
        HurtboxComponent.Hurt += TriggerPickup;
    }

    public async void TriggerPickup(HitboxComponent hitboxComponent)
    {
        HandlePickup(hitboxComponent);
        PulseComponent.StopPulse();

        var tween = CreateTween();
        tween.SetParallel();

        tween.TweenProperty(this, "scale", Scale * 2f, 0.30f);
        tween.TweenProperty(this, "modulate:a", 0.0f, 0.30f);

        ShowFloatingText();
        await ToSignal(tween, "finished");

        QueueFree();
    }

    private void ShowFloatingText()
    {
        if (FloatingTextScene == null) return;

        var textInstance = FloatingTextScene.Instantiate<Label>();
        GetParent().AddChild(textInstance);
        textInstance.GlobalPosition = GlobalPosition;
    }

    public abstract void HandlePickup(HitboxComponent hitboxComponent);
}
