using Godot;

[GlobalClass]
public partial class HurtComponent : Node
{
    [Export] public StatsComponent StatsComponent { get; set; }
    [Export] public HurtboxComponent hurtboxComponent { get; set; }

    public override void _Ready()
    {
        hurtboxComponent.Hurt += OnHurt;
    }

    private void OnHurt(HitboxComponent hitboxComponent)
    {
        StatsComponent.Health -= hitboxComponent.Damage;
    }
}
