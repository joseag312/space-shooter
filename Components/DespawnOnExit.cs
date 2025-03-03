using System.Diagnostics;
using Godot;

[GlobalClass]
public partial class DespawnOnExit : Node
{
	[Export] private NodePath notifierPath;

	private VisibleOnScreenNotifier2D visibleNotifier;

	public override void _Ready()
	{
		if (notifierPath == null)
		{
			Debug.Print("DespawnOnExit: No notifier path assigned!");
			return;
		}

		visibleNotifier = GetNodeOrNull<VisibleOnScreenNotifier2D>(notifierPath);

		if (visibleNotifier != null)
		{
			visibleNotifier.ScreenExited += Despawn;
		}
		else
		{
			Debug.Print("DespawnOnExit: Could not find VisibleOnScreenNotifier2D at " + notifierPath);
		}
	}

	private void Despawn()
	{
		GetParent().QueueFree();
	}
}
