using Godot;
using System.Collections.Generic;

public partial class AutoWeaponInventory : Node
{
    public static AutoWeaponInventory Instance { get; private set; }

    private const string SavePath = "user://savegame_weapons.dat";

    public Dictionary<string, WeaponInstanceData> WeaponStates = new();

    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;
            Load();
        }
        else
        {
            QueueFree();
        }
    }

    public WeaponInstanceData GetWeaponState(string key)
    {
        if (!WeaponStates.TryGetValue(key, out var data))
        {
            data = new WeaponInstanceData(key);
            WeaponStates[key] = data;
        }

        return data;
    }

    public WeaponInstanceData GetInstanceData(string key)
    {
        return GetWeaponState(key);
    }

    public void Save()
    {
        var data = new Godot.Collections.Array<Godot.Collections.Dictionary>();

        foreach (var pair in WeaponStates)
        {
            var dict = new Godot.Collections.Dictionary
            {
                { "key", pair.Value.Key },
                { "amount", pair.Value.CurrentAmount },
                { "cooldown", pair.Value.CooldownRemaining },
                { "override_dmg", pair.Value.OverrideDamage },
                { "override_dmg_pct", pair.Value.OverrideDamagePercentage },
                { "override_cd", pair.Value.OverrideCooldownTime }
            };

            data.Add(dict);
        }

        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
        file.StoreVar(data);
    }

    public void Load()
    {
        WeaponStates.Clear();

        if (!FileAccess.FileExists(SavePath))
        {
            GD.Print("DEBUG: AutoWeaponInventory - No save found. Starting fresh.");
            return;
        }

        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
        var raw = (Godot.Collections.Array)file.GetVar();

        foreach (Godot.Collections.Dictionary entry in raw)
        {
            var key = (string)entry["key"];
            var data = new WeaponInstanceData(key)
            {
                CurrentAmount = (int)entry["amount"],
                CooldownRemaining = (float)entry["cooldown"],
                OverrideDamage = (int)entry["override_dmg"],
                OverrideDamagePercentage = (int)entry["override_dmg_pct"],
                OverrideCooldownTime = (float)entry["override_cd"]
            };

            WeaponStates[key] = data;
        }
    }

    public void Reset()
    {
        WeaponStates.Clear();
        Save();
    }
}
