using Godot;
using System;
using System.Collections.Generic;

public partial class AutoGameStats : Node
{
	public static AutoGameStats Instance { get; private set; }

	[Signal] public delegate void CurrencyChangedEventHandler();

	private const string SavePath = "user://savegame_game.dat";
	private int _pawllars = 0;
	private int _mewnits = 0;

	public Dictionary<string, List<string>> LevelDependencies = new();

	public Dictionary<string, bool> LevelCleared = new();
	public Dictionary<string, bool> DialogSeen = new();
	public int Karma = 0;
	public int LevelMultiplier;
	public float CurrencyMultiplier = 1f;

	public override void _Ready()
	{
		if (Instance == null)
		{
			Instance = this;
			ProcessMode = ProcessModeEnum.Always;

			LevelDependencies = new Dictionary<string, List<string>>
			{
				{ "L3", new List<string> { "L1" } },
				{ "L4", new List<string> { "L1" } },
				{ "L2", new List<string> { "L3" } },
				{ "L5", new List<string> { "L2" , "L4"} }
			};
		}
		else
		{
			QueueFree();
		}
	}

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

	public void Save()
	{
		var clearedLevels = new Godot.Collections.Dictionary<string, Variant>();
		foreach (var pair in LevelCleared)
			clearedLevels[pair.Key] = pair.Value;

		var dialogSeen = new Godot.Collections.Dictionary<string, Variant>();
		foreach (var pair in DialogSeen)
			dialogSeen[pair.Key] = pair.Value;

		var data = new Godot.Collections.Dictionary<string, Variant>
		{
			{ "pawllars", Pawllars },
			{ "mewnits", Mewnits },
			{ "karma", Karma },
			{ "cleared_levels", clearedLevels },
			{ "dialog_seen", dialogSeen }
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

		if (data.ContainsKey("cleared_levels"))
		{
			var clearedLevels = (Godot.Collections.Dictionary)data["cleared_levels"];
			foreach (var key in clearedLevels.Keys)
				LevelCleared[key.ToString()] = (bool)clearedLevels[key];
		}

		if (data.ContainsKey("dialog_seen"))
		{
			var dialogSeen = (Godot.Collections.Dictionary)data["dialog_seen"];
			foreach (var key in dialogSeen.Keys)
				DialogSeen[key.ToString()] = (bool)dialogSeen[key];
		}
	}

	public void Reset()
	{
		Pawllars = 0;
		Mewnits = 0;
		Karma = 0;
		LevelMultiplier = 0;
		CurrencyMultiplier = 1f;

		LevelCleared.Clear();
		DialogSeen.Clear();

		Save();
	}

	public bool IsLevelCleared(string levelId)
	{
		return LevelCleared.TryGetValue(levelId, out var cleared) && cleared;
	}

	public bool HasSeenDialog(string levelId)
	{
		return DialogSeen.TryGetValue(levelId, out var seen) && seen;
	}

	public bool IsLevelAvailable(string levelId)
	{
		if (LevelCleared.ContainsKey(levelId)) return true;

		if (!LevelDependencies.ContainsKey(levelId)) return true;

		foreach (var prereq in LevelDependencies[levelId])
		{
			if (!LevelCleared.TryGetValue(prereq, out var isCleared) || !isCleared)
				return false;
		}

		return true;
	}
}
