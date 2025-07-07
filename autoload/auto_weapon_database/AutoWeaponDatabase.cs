using System;
using System.Collections.Generic;
using Godot;

public partial class AutoWeaponDatabase : Node
{
    public static AutoWeaponDatabase Instance { get; private set; }

    private Dictionary<string, WeaponDataComponent> _weaponMapping;

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
            return;
        }

        _weaponMapping = new Dictionary<string, WeaponDataComponent>();
        LoadWeapons();
    }

    private void LoadWeapons()
    {
        LoadWeaponResourcesFromFolder("res://resources/weapons/");
    }

    private void LoadWeaponResourcesFromFolder(string folderPath)
    {
        var dir = DirAccess.Open(folderPath);

        foreach (string file in dir.GetFiles())
        {
            if (!file.EndsWith(".tres") && !file.EndsWith(".res"))
                continue;

            string fullPath = folderPath + file;
            var weapon = GD.Load<WeaponDataComponent>(fullPath);
            if (weapon == null)
            {
                GD.PrintErr($"ERROR: AutoWeaponDatabase - Failed to load weapon resource: {fullPath}");
                continue;
            }

            if (string.IsNullOrEmpty(weapon.Key))
            {
                GD.PrintErr($"ERROR: AutoWeaponDatabase - Weapon resource missing Key: {fullPath}");
                continue;
            }

            if (_weaponMapping.ContainsKey(weapon.Key))
            {
                GD.PrintErr($"ERROR: AutoWeaponDatabase - Duplicate weapon key: {weapon.Key}");
                continue;
            }

            _weaponMapping[weapon.Key] = weapon;
        }

        GD.Print($"DEBUG: AutoWeaponDatabase - Loaded {_weaponMapping.Count} weapons.");
    }

    public WeaponDataComponent GetWeaponData(string key)
    {
        if (!_weaponMapping.TryGetValue(key, out var data))
        {
            GD.PrintErr($"ERROR: AutoWeaponDatabase - Weapon ID '{key}' not found");
            return null;
        }

        return data;
    }

    public List<string> GetAllWeaponKeys()
    {
        return new List<string>(_weaponMapping.Keys);
    }

    public IEnumerable<WeaponDataComponent> GetAllWeapons()
    {
        return _weaponMapping.Values;
    }

    public IEnumerable<WeaponDataComponent> GetWeaponsBySlotType(WeaponDataComponent.WeaponSlotType slotType)
    {
        foreach (var weapon in _weaponMapping.Values)
        {
            if (weapon.SlotType == slotType)
                yield return weapon;
        }
    }
}
