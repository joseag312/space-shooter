using Godot;

[GlobalClass]
public partial class DespawnOnExitComponent : Node
{
	[Export] VisibleOnScreenNotifier2D visibleNotifier;

	public override void _Ready()
	{
		if (visibleNotifier == null)
		{
			GD.PrintErr("ERROR: DespawnOnExitComponent - No notifier assigned");
			return;
		}
		visibleNotifier.ScreenExited += Despawn;
	}

	public override void _Process(double delta)
	{

	}

	private void Despawn()
	{
		GetParent().QueueFree();
	}
}