using Godot;

[GlobalClass]
public partial class HurtComponent : Node
{
    [Export] public StatsComponent StatsComponent { get; set; }
    [Export] public HurtboxComponent HurtboxComponent { get; set; }
    [Export] public InvincibilityComponent InvincibilityComponent { get; set; }

    public override void _Ready()
    {
        if (HurtboxComponent != null)
            HurtboxComponent.Hurt += OnHurt;
    }

    private void OnHurt(HitboxComponent hitboxComponent)
    {
        if (InvincibilityComponent != null && InvincibilityComponent.IsInvincible)
            return;

        StatsComponent.Health -= hitboxComponent.Damage;
        InvincibilityComponent?.StartInvincibility();
    }
}
