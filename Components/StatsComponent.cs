using Godot;

[GlobalClass]
public partial class StatsComponent : Node
{
    [Signal] public delegate void HealthChangedEventHandler();
    [Signal] public delegate void NoHealthEventHandler();

    private int _health = 1;

    [Export]
    public int Health
    {
        get => _health;
        set
        {
            _health = value;
            EmitSignal(SignalName.HealthChanged);
            if (_health == 0)
                EmitSignal(SignalName.NoHealth);
        }
    }
}
