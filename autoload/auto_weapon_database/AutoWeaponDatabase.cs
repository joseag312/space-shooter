using System;
using System.Collections.Generic;
using Godot;

public partial class AutoWeaponDatabase : Node
{
    public static AutoWeaponDatabase Instance { get; private set; }

    public WeaponDataComponent BasicWeapon { get; private set; }
    public WeaponDataComponent LargeWeapon { get; private set; }
    public WeaponDataComponent[] SpecialWeapons { get; private set; }

    private Dictionary<String, WeaponDataComponent> _weaponMapping;

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
        SpecialWeapons = new WeaponDataComponent[4];
        _weaponMapping = new Dictionary<string, WeaponDataComponent>();
        LoadWeapons();
    }

    private void LoadWeapons()
    {
        LoadWeaponResourcesFromFolder("res://resources/weapons/");

        BasicWeapon = GetWeaponData("PPBasicBlue");
        LargeWeapon = GetWeaponData("PPBigBlue");

        for (int i = 0; i < SpecialWeapons.Length; i++)
        {
            SpecialWeapons[i] = GetWeaponData("PPBigBlue");
        }
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
    }

    public WeaponDataComponent GetWeaponData(string id)
    {
        if (!_weaponMapping.TryGetValue(id, out var data))
        {
            GD.PrintErr($"ERROR: AutoWeaponDatabase - Weapon ID '{id}' not found");
            return null;
        }

        return data;
    }

    public EffectiveWeaponData GetEffectiveWeaponData(string key)
    {
        var baseData = GetWeaponData(key);
        var instance = G.WI.GetWeaponState(key);

        if (baseData == null || instance == null)
            return null;

        return new EffectiveWeaponData
        {
            Key = key,
            Damage = instance.HasOverrideDamage ? instance.OverrideDamage : baseData.Damage,
            DamagePercentage = instance.HasOverrideDamagePercentage ? instance.OverrideDamagePercentage : baseData.DamagePercentage,
            CooldownTime = instance.HasOverrideCooldown ? instance.OverrideCooldownTime : baseData.CooldownTime,
            ProjectilePath = baseData.ProjectilePath,
            SpawnLocation = baseData.SpawnLocation,
            BaseData = baseData,
            InstanceData = instance
        };
    }

}
