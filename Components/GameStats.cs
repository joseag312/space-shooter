using Godot;

[GlobalClass]
public partial class GameStats : Node
{
	public static GameStats Instance { get; private set; }

	[Export] public int Score { get; set; } = 0;
	[Export] public int HighScore { get; set; } = 0;

	public override void _Ready()
	{
		if (Instance == null)
		{
			Instance = this;
			GetTree().Root.AddChild(this);
			ProcessMode = ProcessModeEnum.Always;
		}
		else
		{
			QueueFree();
		}
	}
}
