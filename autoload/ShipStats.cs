using Godot;

public partial class ShipStats : Node
{
	public static ShipStats Instance { get; private set; }

	[Export] public int Health { get; set; } = 25;
	[Export] public int Speed { get; set; } = 250;

	public override void _Ready()
	{
		if (Instance == null)
		{
			Instance = this;
			ProcessMode = ProcessModeEnum.Always;
		}
		else
		{
			QueueFree();
		}
	}
}