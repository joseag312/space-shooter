using System;
using System.Collections.Generic;
using Godot;

public partial class WeaponDatabase : Node
{
    public static WeaponDatabase Instance { get; private set; }
    public WeaponDataComponent BasicWeapon { get; private set; }
    public WeaponDataComponent LargeWeapon { get; private set; }
    public WeaponDataComponent[] SpecialWeapons { get; private set; }
    private Dictionary<String, WeaponDataComponent> weaponMapping;

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
        weaponMapping = new Dictionary<String, WeaponDataComponent>();
        LoadWeapons();
    }

    private void LoadWeapons()
    {
        LoadWeaponData();
        BasicWeapon = weaponMapping["BasicBlueLaser"];
        LargeWeapon = weaponMapping["BigBlueLaser"];
        SpecialWeapons[0] = weaponMapping["BigBlueLaser"];
        SpecialWeapons[1] = weaponMapping["BigBlueLaser"];
        SpecialWeapons[2] = weaponMapping["BigBlueLaser"];
        SpecialWeapons[3] = weaponMapping["BigBlueLaser"];
    }

    public void LoadWeaponData()
    {
        WeaponDataComponent basicLaser = new WeaponDataComponent();
        basicLaser.damage = 1;
        basicLaser.damagePercentage = 0;
        basicLaser.cooldownTime = 0.2f;
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
        weaponMapping.Add(basicLaser.projectileName, basicLaser);
        weaponMapping.Add(bigLaser.projectileName, bigLaser);
    }

    public WeaponDataComponent GetWeaponData(String id)
    {
        return weaponMapping[id];
    }
}
