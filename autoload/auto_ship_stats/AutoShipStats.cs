using Godot;

public partial class AutoShipStats : Node
{
	public static AutoShipStats Instance { get; private set; }

	[Export] private int _health = 25;
	[Export] private int _speed = 250;

	private const string SavePath = "user://savegame_ship.dat";

	public int Health
	{
		get => _health;
		set => _health = value;
	}

	public int Speed
	{
		get => _speed;
		set => _speed = value;
	}

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
			{ "health", _health },
			{ "speed", _speed }
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

		_health = (int)data["health"];
		_speed = (int)data["speed"];
	}

	public void Reset()
	{
		_health = 25;
		_speed = 250;
		Save();
	}
}
