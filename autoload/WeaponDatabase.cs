using System;
using System.Collections.Generic;
using Godot;

public partial class WeaponDatabase : Node
{
    public static WeaponDatabase Instance { get; private set; }

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
        BasicWeapon = _weaponMapping["BasicBlueLaser"];
        LargeWeapon = _weaponMapping["BigBlueLaser"];
        SpecialWeapons[0] = _weaponMapping["BigBlueLaser"];
        SpecialWeapons[1] = _weaponMapping["BigBlueLaser"];
        SpecialWeapons[2] = _weaponMapping["BigBlueLaser"];
        SpecialWeapons[3] = _weaponMapping["BigBlueLaser"];
    }

    public void LoadWeaponData()
    {
        WeaponDataComponent basicLaser = new WeaponDataComponent();
        basicLaser.damage = 1;
        basicLaser.damagePercentage = 0;
        basicLaser.cooldownTime = 0.3f;
        basicLaser.spawnLocation = 2;
        basicLaser.projectileName = "BasicBlueLaser";
        basicLaser.projectilePath = "res://player_projectiles/basic_blue_laser/basic_blue_laser.tscn";
        WeaponDataComponent bigLaser = new WeaponDataComponent();
        bigLaser.damage = 5;
        bigLaser.damagePercentage = 0;
        bigLaser.cooldownTime = 0.5f;
        bigLaser.spawnLocation = 3;
        bigLaser.projectileName = "BigBlueLaser";
        bigLaser.projectilePath = "res://player_projectiles/big_blue_laser/big_blue_laser.tscn";
        _weaponMapping.Add(basicLaser.projectileName, basicLaser);
        _weaponMapping.Add(bigLaser.projectileName, bigLaser);
    }

    public WeaponDataComponent GetWeaponData(string id)
    {
        return _weaponMapping[id];
    }
}
