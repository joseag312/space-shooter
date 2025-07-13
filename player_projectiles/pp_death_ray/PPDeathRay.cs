using Godot;

[GlobalClass]
public partial class PPDeathRay : Node2D
{
    [Export] public Timer LifetimeTimer;

    public override void _Ready()
    {
        if (LifetimeTimer == null)
        {
            GD.PrintErr("ERROR: PPDeathRay - LifetimeTimer not set");
            QueueFree();
            return;
        }

        LifetimeTimer.Timeout += OnLifetimeExpired;
        LifetimeTimer.Start();
    }

    private void OnLifetimeExpired()
    {
        QueueFree();
    }
}
