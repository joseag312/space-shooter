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
            Reset();
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
        EquipSlotWeapon(1, "PPMissileMach");
        EquipSlotWeapon(2, "PPTeleport");
        EquipSlotWeapon(3, "PPShield");
        EquipSlotWeapon(4, "PPDeathRay");
    }

    private WeaponStateComponent DefaultWeaponState(string key)
    {
        var baseData = AutoWeaponDatabase.Instance.GetWeaponData(key);
        if (baseData == null)
        {
            GD.PrintErr($"ERROR: AutoWeaponInventory - No base data found for weapon key '{key}'");
            return new WeaponStateComponent(key);
        }

        bool isSlotWeapon = baseData.SlotType == WeaponDataComponent.WeaponSlotType.Slot;

        var state = new WeaponStateComponent(key)
        {
            BaseData = baseData,
            CurrentAmount = isSlotWeapon ? 5 : 0,
            CooldownRemaining = 0f,
            OverrideSpeed = -1,
            OverrideDamage = -1,
            OverrideDamagePercentage = -1,
            OverrideCooldownTime = -1f,
            OverrideMaxAmount = isSlotWeapon ? -1 : 0,
            OverrideUnlocked = false,
            UseOverrideUnlocked = false
        };

        return state;
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
                { "override_speed", pair.Value.OverrideSpeed},
                { "override_dmg", pair.Value.OverrideDamage },
                { "override_dmg_pct", pair.Value.OverrideDamagePercentage },
                { "override_cd", pair.Value.OverrideCooldownTime },
                { "override_max", pair.Value.OverrideMaxAmount },
                { "override_unlocked", pair.Value.OverrideUnlocked },
                { "use_override_unlocked", pair.Value.UseOverrideUnlocked }
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
            GD.Print("DEBUG: No save found. Calling DefaultWeaponEquip() and Save()");
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
                    OverrideSpeed = (int)entry["override_speed"],
                    OverrideDamage = (int)entry["override_dmg"],
                    OverrideDamagePercentage = (int)entry["override_dmg_pct"],
                    OverrideCooldownTime = (float)entry["override_cd"],
                    OverrideMaxAmount = (int)entry.GetValueOrDefault("override_max", -1),
                    OverrideUnlocked = (bool)entry.GetValueOrDefault("override_unlocked", false),
                    UseOverrideUnlocked = (bool)entry.GetValueOrDefault("use_override_unlocked", false)
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
            GD.Print("DEBUG: No equipped section in save. Calling DefaultWeaponEquip().");
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

    public void DebugPrintSaveContents()
    {
        if (!FileAccess.FileExists(SavePath))
        {
            GD.Print("DEBUG: AutoWeaponInventory - No save file found.");
            return;
        }

        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
        var root = (Godot.Collections.Dictionary)file.GetVar();

        GD.Print("DEBUG: AutoWeaponInventory - Save File Contents:");

        if (root.TryGetValue("weapons", out var weaponsRaw))
        {
            GD.Print("  Weapons:");
            foreach (Godot.Collections.Dictionary weapon in (Godot.Collections.Array)weaponsRaw)
            {
                GD.Print($"    Key: {weapon.GetValueOrDefault("key", "UNKNOWN")}");
                GD.Print($"      Amount: {weapon.GetValueOrDefault("amount", -1)}");
                GD.Print($"      Cooldown: {weapon.GetValueOrDefault("cooldown", -1f)}");
                GD.Print($"      Override Speed: {weapon.GetValueOrDefault("override_speed", -1)}");
                GD.Print($"      Override Damage: {weapon.GetValueOrDefault("override_dmg", -1)}");
                GD.Print($"      Override %: {weapon.GetValueOrDefault("override_dmg_pct", -1)}");
                GD.Print($"      Override CD: {weapon.GetValueOrDefault("override_cd", -1f)}");
                GD.Print($"      Override Max: {weapon.GetValueOrDefault("override_max", -1)}");
                GD.Print($"      Override Unlocked: {weapon.GetValueOrDefault("override_unlocked", false)}");
                GD.Print($"      Use Override Unlocked: {weapon.GetValueOrDefault("use_override_unlocked", false)}");
            }
        }
        else
        {
            GD.Print("  Weapons: NONE");
        }

        if (root.TryGetValue("equipped", out var equippedRaw))
        {
            GD.Print("  Equipped:");
            foreach (string slot in ((Godot.Collections.Dictionary)equippedRaw).Keys)
            {
                string value = (string)((Godot.Collections.Dictionary)equippedRaw)[slot];
                GD.Print($"    {slot}: {(string.IsNullOrEmpty(value) ? "EMPTY" : value)}");
            }
        }
        else
        {
            GD.Print("  Equipped: NONE");
        }
    }
}
