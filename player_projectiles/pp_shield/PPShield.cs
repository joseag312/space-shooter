using Godot;
using System;

[GlobalClass]
public partial class PPShield : Node2D
{
    [Export] public HurtboxComponent Hurtbox;
    [Export] public Timer Timer;

    private InvincibilityComponent _invincibilityComponent;
    private int _hitCount = 0;
    private const int MaxHits = 5;

    public override void _Ready()
    {
        if (Hurtbox == null)
        {
            GD.PrintErr("ERROR: PPShield - Hurtbox not set");
            QueueFree();
            return;
        }

        Hurtbox.Connect(HurtboxComponent.SignalName.Hurt, new Callable(this, nameof(OnHitReceived)));

        _invincibilityComponent = GetParent().GetNodeOrNull<InvincibilityComponent>("InvincibilityComponent");
        if (_invincibilityComponent == null)
        {
            GD.PrintErr("ERROR: PPShield - InvincibilityComponent not found on parent");
            QueueFree();
            return;
        }

        Timer.Timeout += Despawn;
        Timer.Start();

        _invincibilityComponent.ForceStartInvincibility();
    }


    private void OnHitReceived(HitboxComponent hitbox)
    {
        _hitCount++;

        if (_hitCount >= MaxHits)
            Despawn();
    }

    private void Despawn()
    {
        if (_invincibilityComponent != null)
            _invincibilityComponent.ForceEndInvincibility();

        QueueFree();
    }
}
