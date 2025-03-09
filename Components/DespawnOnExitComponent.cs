using Godot;

[GlobalClass]
public partial class DespawnOnExitComponent : Node
{
	[Export] VisibleOnScreenNotifier2D visibleNotifier;

	public override void _Ready()
	{
		if (visibleNotifier == null)
		{
			GD.PrintErr("ERROR: DespawnOnExitComponet - No notifier assigned");
			return;
		}
		visibleNotifier.ScreenExited += Despawn;
	}

	private void Despawn()
	{
		GetParent().QueueFree();
	}
}
