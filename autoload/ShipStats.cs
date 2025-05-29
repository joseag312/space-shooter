using Godot;

public partial class ShipStats : Node
{
	public static ShipStats Instance { get; private set; }

	[Export] public int Health { get; set; } = 25;
	[Export] public int Speed { get; set; } = 250;

	private const string SavePath = "user://savegame_ship.dat";

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

	public void Save()
	{
		var data = new Godot.Collections.Dictionary<string, Variant>
		{
			{ "health", Health },
			{ "speed", Speed }
		};

		using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
		file.StoreVar(data);
	}

	public void Load()
	{
		if (!FileAccess.FileExists(SavePath))
		{
			Save();
			return;
		}

		using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
		var data = (Godot.Collections.Dictionary)file.GetVar();

		Health = (int)data["health"];
		Speed = (int)data["speed"];
	}
}