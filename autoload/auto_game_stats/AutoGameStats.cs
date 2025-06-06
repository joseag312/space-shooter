using Godot;

public partial class AutoGameStats : Node
{
	public static AutoGameStats Instance { get; private set; }

	[Export] private int _pawllars = 0;
	[Export] private int _mewnits = 0;

	public int Pawllars
	{
		get => _pawllars;
		set => _pawllars = value;
	}

	public int Mewnits
	{
		get => _mewnits;
		set => _mewnits = value;
	}

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
			{ "pawllars", _pawllars },
			{ "mewnits", _mewnits }
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

		_pawllars = (int)data["pawllars"];
		_mewnits = (int)data["mewnits"];
	}
}
