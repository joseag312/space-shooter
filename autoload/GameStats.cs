using Godot;

public partial class GameStats : Node
{
	public static GameStats Instance { get; private set; }

	[Export] public int Pawllars { get; set; } = 0;
	[Export] public int Mewnits { get; set; } = 0;

	private const string SavePath = "user://savegame_game.dat";

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
			{ "pawllars", Pawllars },
			{ "mewnits", Mewnits }
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

		Pawllars = (int)data["pawllars"];
		Mewnits = (int)data["mewnits"];
	}
}
