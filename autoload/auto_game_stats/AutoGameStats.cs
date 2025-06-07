using Godot;

public partial class AutoGameStats : Node
{
	public static AutoGameStats Instance { get; private set; }

	public int Pawllars = 0;
	public int Mewnits = 0;

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
