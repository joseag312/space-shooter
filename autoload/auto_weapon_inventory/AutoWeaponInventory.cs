using Godot;
using System.Collections.Generic;

public partial class AutoWeaponInventory : Node
{
    public static AutoWeaponInventory Instance { get; private set; }
    private const string SavePath = "user://savegame_weapons.dat";

    private Dictionary<string, WeaponStateComponent> _weaponStates = new();
    private Dictionary<string, string> _equippedWeapons = new();

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

    private void AddWeaponState(string key)
    {
        if (!_weaponStates.ContainsKey(key))
        {
            _weaponStates[key] = DefaultWeaponState(key);
        }
    }

    public WeaponStateComponent GetWeaponState(string key)
    {
        if (_weaponStates.TryGetValue(key, out var data))
            return data;

        var instance = DefaultWeaponState(key);
        _weaponStates[key] = instance;
        return instance;
    }

    public string GetEquippedWeaponKey(string slot)
    {
        return _equippedWeapons.TryGetValue(slot, out var key) ? key : null;
    }

    public WeaponStateComponent GetEquippedWeaponState(string slot)
    {
        string key = GetEquippedWeaponKey(slot);
        return key != null ? GetWeaponState(key) : null;
    }

    public void EquipBasicWeapon(string weaponKey)
    {
        var data = AutoWeaponDatabase.Instance.GetWeaponData(weaponKey);
        if (data == null || data.SlotType != WeaponDataComponent.WeaponSlotType.Basic)
        {
            GD.PrintErr($"ERROR: AutoWeaponInventory - '{weaponKey}' is not a Basic weapon");
            return;
        }

        _equippedWeapons[WeaponSlotNames.Basic] = weaponKey;
        AddWeaponState(weaponKey);
    }

    public void EquipBigWeapon(string weaponKey)
    {
        var data = AutoWeaponDatabase.Instance.GetWeaponData(weaponKey);
        if (data == null || data.SlotType != WeaponDataComponent.WeaponSlotType.Big)
        {
            GD.PrintErr($"ERROR: AutoWeaponInventory - '{weaponKey}' is not a Big weapon");
            return;
        }

        _equippedWeapons[WeaponSlotNames.Big] = weaponKey;
        AddWeaponState(weaponKey);
    }

    public void EquipSlotWeapon(int index, string weaponKey)
    {
        if (index < 1 || index > 4)
        {
            GD.PrintErr("ERROR: AutoWeaponInventory - Slot index must be 1–4");
            return;
        }

        string targetSlot = WeaponSlotNames.GetSpecialSlot(index);

        if (weaponKey == null)
        {
            _equippedWeapons[targetSlot] = null;
            return;
        }

        var data = AutoWeaponDatabase.Instance.GetWeaponData(weaponKey);
        if (data == null || data.SlotType != WeaponDataComponent.WeaponSlotType.Slot)
        {
            GD.PrintErr($"ERROR: AutoWeaponInventory - '{weaponKey}' is not a Slot weapon");
            return;
        }

        foreach (var kvp in _equippedWeapons)
        {
            if (kvp.Key != targetSlot && kvp.Value == weaponKey && kvp.Key.StartsWith("special_"))
            {
                _equippedWeapons[kvp.Key] = null;
            }
        }

        _equippedWeapons[targetSlot] = weaponKey;
        AddWeaponState(weaponKey);
    }

    private void DefaultWeaponEquip()
    {
        EquipBasicWeapon("PPBasicBlue");
        EquipBigWeapon("PPBigBlue");
        EquipSlotWeapon(1, null);
        EquipSlotWeapon(2, null);
        EquipSlotWeapon(3, null);
        EquipSlotWeapon(4, null);
    }

    private WeaponStateComponent DefaultWeaponState(string key)
    {
        var baseData = AutoWeaponDatabase.Instance.GetWeaponData(key);
        if (baseData == null)
        {
            GD.PrintErr($"ERROR: AutoWeaponInventory - No base data found for weapon key '{key}'");
            return new WeaponStateComponent(key);
        }

        return new WeaponStateComponent(key)
        {
            BaseData = baseData,
            CurrentAmount = baseData.Infinite ? int.MaxValue : baseData.MaxAmount,
            CooldownRemaining = 0f,
            OverrideDamage = -1,
            OverrideDamagePercentage = -1,
            OverrideCooldownTime = -1f
        };
    }

    public void Save()
    {
        var weaponArray = new Godot.Collections.Array<Godot.Collections.Dictionary>();
        foreach (var pair in _weaponStates)
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
            weaponArray.Add(dict);
        }

        var equippedDict = new Godot.Collections.Dictionary();
        foreach (var pair in _equippedWeapons)
        {
            equippedDict[pair.Key] = pair.Value ?? "";
        }

        var root = new Godot.Collections.Dictionary
        {
            { "weapons", weaponArray },
            { "equipped", equippedDict }
        };

        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
        file.StoreVar(root);
    }

    public void Load()
    {
        _weaponStates.Clear();
        _equippedWeapons.Clear();

        if (!FileAccess.FileExists(SavePath))
        {
            GD.Print("DEBUG: AutoWeaponInventory - No save found. Starting fresh.");
            DefaultWeaponEquip();
            Save();
            return;
        }

        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
        var root = (Godot.Collections.Dictionary)file.GetVar();

        if (root.TryGetValue("weapons", out var weaponsRaw))
        {
            foreach (Godot.Collections.Dictionary entry in (Godot.Collections.Array)weaponsRaw)
            {
                var key = (string)entry["key"];
                var baseData = AutoWeaponDatabase.Instance.GetWeaponData(key);
                if (baseData == null)
                {
                    GD.PrintErr($"ERROR: AutoWeaponInventory - Missing base data for '{key}'");
                    continue;
                }

                var data = new WeaponStateComponent(key)
                {
                    BaseData = baseData,
                    CurrentAmount = (int)entry["amount"],
                    CooldownRemaining = (float)entry["cooldown"],
                    OverrideDamage = (int)entry["override_dmg"],
                    OverrideDamagePercentage = (int)entry["override_dmg_pct"],
                    OverrideCooldownTime = (float)entry["override_cd"]
                };

                _weaponStates[key] = data;
            }
        }

        if (root.TryGetValue("equipped", out var equippedRaw))
        {
            foreach (string slot in ((Godot.Collections.Dictionary)equippedRaw).Keys)
            {
                string value = (string)((Godot.Collections.Dictionary)equippedRaw)[slot];
                _equippedWeapons[slot] = string.IsNullOrEmpty(value) ? null : value;
            }
        }
        else
        {
            GD.Print("DEBUG: AutoWeaponInventory - Seeding default weapon assignment");
            DefaultWeaponEquip();
        }
    }

    public void Reset()
    {
        _weaponStates.Clear();
        _equippedWeapons.Clear();
        DefaultWeaponEquip();
        Save();
    }
}