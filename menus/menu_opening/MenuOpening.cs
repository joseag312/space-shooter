using Godot;
using System;

public partial class MenuOpening : Control
{
    [Export] public AnimationPlayer AnimationPlayer { get; set; }

    public override void _Ready()
    {
        PlayOpening();
    }

    public void PlayOpening()
    {
        AnimationPlayer.Play("OpeningMenu/Opening");
    }

    public void MainMenuLoad()
    {
        GetTree().ChangeSceneToFile("res://menus/menu_main/menu_main.tscn");
    }
}
