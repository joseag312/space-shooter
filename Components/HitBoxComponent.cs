using Godot;

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

        // Get InvincibilityComponent from the parent of the hurtbox
        InvincibilityComponent invincibility = hurtbox.GetParent().GetNodeOrNull<InvincibilityComponent>("InvincibilityComponent");

        // If the target has invincibility and is currently invincible, ignore the hit
        if (invincibility != null && invincibility.IsInvincible)
            return;

        EmitSignal(SignalName.HitHurtbox, hurtbox);
        hurtbox.EmitSignal(HurtboxComponent.SignalName.Hurt, this);
    }
}
