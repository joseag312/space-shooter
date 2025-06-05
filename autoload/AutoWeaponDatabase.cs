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
        LoadWeaponData();
        BasicWeapon = _weaponMapping["PPBasicBlue"];
        LargeWeapon = _weaponMapping["PPBigBlue"];
        SpecialWeapons[0] = _weaponMapping["PPBigBlue"];
        SpecialWeapons[1] = _weaponMapping["PPBigBlue"];
        SpecialWeapons[2] = _weaponMapping["PPBigBlue"];
        SpecialWeapons[3] = _weaponMapping["PPBigBlue"];
    }

    public void LoadWeaponData()
    {
        WeaponDataComponent basicLaser = new WeaponDataComponent();
        basicLaser.Damage = 1;
        basicLaser.DamagePercentage = 0;
        basicLaser.CooldownTime = 0.3f;
        basicLaser.SpawnLocation = 2;
        basicLaser.ProjectileName = "PPBasicBlue";
        basicLaser.ProjectilePath = "res://player_projectiles/pp_basic_blue/pp_basic_blue.tscn";
        WeaponDataComponent bigLaser = new WeaponDataComponent();
        bigLaser.Damage = 5;
        bigLaser.DamagePercentage = 0;
        bigLaser.CooldownTime = 0.5f;
        bigLaser.SpawnLocation = 3;
        bigLaser.ProjectileName = "PPBigBlue";
        bigLaser.ProjectilePath = "res://player_projectiles/pp_big_blue/pp_big_blue.tscn";
        _weaponMapping.Add(basicLaser.ProjectileName, basicLaser);
        _weaponMapping.Add(bigLaser.ProjectileName, bigLaser);
    }

    public WeaponDataComponent GetWeaponData(string id)
    {
        return _weaponMapping[id];
    }
}
