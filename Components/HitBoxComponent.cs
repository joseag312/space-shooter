using Godot;
using System;

[GlobalClass]
public partial class HitboxComponent : Area2D
{
    [Export] public float Damage { get; set; } = 1.0f;

    [Signal] public delegate void HitHurtboxEventHandler(HurtboxComponent hurtbox);

    public override void _Ready()
    {
        AreaEntered += OnHurtboxEntered;
    }

    private void OnHurtboxEntered(Area2D area)
    {
        if (area is not HurtboxComponent hurtbox) return;
        if (hurtbox.IsInvincible) return;

        EmitSignal(SignalName.HitHurtbox, hurtbox);
        hurtbox.EmitSignal(HurtboxComponent.SignalName.Hurt, this);
    }
}
