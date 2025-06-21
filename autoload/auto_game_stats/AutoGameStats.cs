using Godot;
using System;

public partial class AutoGameStats : Node
{
	public static AutoGameStats Instance { get; private set; }

	[Signal] public delegate void CurrencyChangedEventHandler();

	private int _pawllars = 0;
	private int _mewnits = 0;

	public int Pawllars
	{
		get => _pawllars;
		set
		{
			if (_pawllars != value)
			{
				_pawllars = value;
				EmitSignal(SignalName.CurrencyChanged);
			}
		}
	}

	public int Mewnits
	{
		get => _mewnits;
		set
		{
			if (_mewnits != value)
			{
				_mewnits = value;
				EmitSignal(SignalName.CurrencyChanged);
			}
		}
	}

	public int Karma = 0;

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
			{ "mewnits", Mewnits },
			{ "karma", Karma }
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

		Pawllars = data.ContainsKey("pawllars") ? (int)data["pawllars"] : 0;
		Mewnits = data.ContainsKey("mewnits") ? (int)data["mewnits"] : 0;
		Karma = data.ContainsKey("karma") ? (int)data["karma"] : 0;
	}
}
