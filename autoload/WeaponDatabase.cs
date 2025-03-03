using System.Diagnostics;
using System.Reflection;
using Godot;

[GlobalClass]
public partial class WeaponDatabase : Node
{
    public static WeaponDatabase Instance { get; private set; }

    public PackedScene BasicWeapon { get; private set; }
    public PackedScene LargeWeapon { get; private set; }
    public PackedScene[] SpecialWeapons { get; private set; } = new PackedScene[4];

    public override void _Ready()
    {
        Instance = this;
        LoadWeapons();
    }

    private void LoadWeapons()
    {
        string basicWeaponPath = "res://player_projectiles/basic_blue_laser.tscn";
        string largeWeaponPath = "res://player_projectiles/basic_blue_laser.tscn";

        GD.Print("DEBUG: Loading weapons...");
        if (!ResourceLoader.Exists(basicWeaponPath) || !ResourceLoader.Exists(largeWeaponPath))
        {
            GD.PrintErr("ERROR: basicWeaponPath or largeWeaponPath missing.");
            return;
        }
        BasicWeapon = (PackedScene)ResourceLoader.Load(basicWeaponPath);
        LargeWeapon = (PackedScene)ResourceLoader.Load(largeWeaponPath);

        GD.Print($"DEBUG: Basic Weapon loaded.");
        GD.Print($"DEBUG: Large Weapon loaded.");

        for (int i = 0; i < 4; i++)
        {
            string specialWeaponPath = "res://player_projectiles/basic_blue_laser.tscn";
            SpecialWeapons[i] = (PackedScene)ResourceLoader.Load(specialWeaponPath);
            if (SpecialWeapons[i] == null)
            {
                GD.PrintErr($"ERROR: specialWeaponPath[{i}] missing.");
            }
            else
            {
                GD.Print($"DEBUG: Special Weapon [{i}] loaded.");
            }

        }
    }

}
